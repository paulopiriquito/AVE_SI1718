using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace HtmlReflect
{
    public interface IHtmlGetter
    {
        string GetHtml(object obj, string defaultTemplate, bool isTable);

		string GetHtml(object obj);
	}
    public abstract class HtmlGetter : IHtmlGetter {
	    private HtmlEmit htmlEmit;

	    public HtmlGetter(HtmlEmit htmlEmit) {
		    this.htmlEmit = htmlEmit;
	    }
	    
        public string FormatHtml(string name, object val, string format, bool isTable)
        {
            var value = val == null ? "" : val.ToString();
            string ret = format.Replace("{name}", name).Replace("{value}", value);
            if (isTable)
                return "<td>" + ret + "</td>";
            return ret;
        }
        public abstract string GetHtml(object target, string defaultTemplate, bool isTable);
	    public string GetHtml(object obj) {
		    throw new NotImplementedException();
	    }
    }
    public class HtmlEmit
    {
        static readonly MethodInfo formatterForHtml = typeof(HtmlGetter).GetMethod("FormatHtml", new Type[] { typeof(String), typeof(object), typeof(String), typeof(bool) });
        static readonly MethodInfo concat = typeof(String).GetMethod("Concat", new Type[] { typeof(string), typeof(string) });

        private static Dictionary<Type, IHtmlGetter> htmlTypes = new Dictionary<Type, IHtmlGetter>(); //saves an IHtmlGetter for a Type
        private static Dictionary<Type, PropertyInfo[]> TypeProperties = new Dictionary<Type, PropertyInfo[]>(); //saves a PropertyInfo[] for a Type
        private static Dictionary<PropertyInfo, HtmlAs> HtmlAsDict = new Dictionary<PropertyInfo, HtmlAs>(); // saves property HtmlAs attributes
        private static List<PropertyInfo> htmlIgnoreList = new List<PropertyInfo>(); // saves property HtmlIgnore attributes
        private static Dictionary<Type, IHtmlGetter> HtmlFormatterTypes = new Dictionary<Type, IHtmlGetter>(); //saves an IHtmlGetter for a Type

        //Inits HtmlIgnoreList and HtmlAsDict for a set of PropertyInfo to prevent repeated reflection on getting property attributes
        private static void HtmlAttributes_init(PropertyInfo[] properties)
        {
            foreach (var property in properties)
            {
                if (!htmlIgnoreList.Contains(property) && !HtmlAsDict.ContainsKey(property))
                {
                    HtmlIgnore ignore = (HtmlIgnore)property.GetCustomAttribute(typeof(HtmlIgnore));
                    if (ignore == null)
                    {
                        HtmlAs htmlAs = (HtmlAs)property.GetCustomAttribute(typeof(HtmlAs));
                        if (htmlAs != null)
                        {
                            HtmlAsDict.Add(property, htmlAs);
                        }
                    }
                    else
                        htmlIgnoreList.Add(property);
                }
            }
        }
        
        //Checks and updates dictionary to prevent use of repeated reflection on Type.GetProperties()
        private static PropertyInfo[] GetPropertyInfoArray(Type type)
        {
            if (!TypeProperties.TryGetValue(type, out var properties))
            {
                properties = type.GetProperties();
                TypeProperties.Add(type, properties);
            }
            return properties;
        }

        //Emits an IHtmlGetter for a Type
        private IHtmlGetter EmitHtmlGetter(Type klass)
        {
            AssemblyName assemblyName = new AssemblyName(klass.Name + "DynamicHtmlGetter");
            AssemblyBuilder assemblyBuilder =
                AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder moduleBuilder =
                assemblyBuilder.DefineDynamicModule(assemblyName.Name, assemblyName.Name + ".dll");
            TypeBuilder typeBuilder = moduleBuilder.DefineType(klass.Name + "HtmlGetter", TypeAttributes.Public, typeof(HtmlGetter));

            EmitCtor(klass ,typeBuilder);

            EmitGetHtml(klass, typeBuilder);

            Type t = typeBuilder.CreateType();
            assemblyBuilder.Save(assemblyName.Name + ".dll");
            return (IHtmlGetter)Activator.CreateInstance(t, new object[] { this });
        }

        public static void EmitCtor(Type klass, TypeBuilder typeBuilder)
        {
            Type[] parameterTypes = { typeof(HtmlEmit) };

            ConstructorBuilder ctor = typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                parameterTypes);

            ILGenerator il = ctor.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Call, typeof(HtmlGetter).GetConstructor(parameterTypes));
            il.Emit(OpCodes.Ret);
        }

        public static void EmitGetHtml(Type klass, TypeBuilder typeBuilder)
        {
            MethodBuilder getHtmlMethodBuilder = typeBuilder.DefineMethod("GetHtml",
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.ReuseSlot,
                typeof(string),
                new Type[] {typeof(object), typeof(string), typeof(bool)}
            );

            ILGenerator il = getHtmlMethodBuilder.GetILGenerator();

            PropertyInfo[] propertyInfoArray = GetPropertyInfoArray(klass);
            HtmlAttributes_init(propertyInfoArray);

            LocalBuilder target = il.DeclareLocal(klass);
            il.Emit(OpCodes.Ldarg_1); // push target
            il.Emit(OpCodes.Castclass, klass); // castclass
            il.Emit(OpCodes.Stloc, target); // store on local variable 


            il.Emit(OpCodes.Ldstr, "");

            foreach (PropertyInfo propertyInfo in propertyInfoArray)
            {
                if (htmlIgnoreList.Contains(propertyInfo)) continue;

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldstr, propertyInfo.Name); // push on stack the property name
                il.Emit(OpCodes.Ldloc, target); // ldloc target
                il.Emit(OpCodes.Call, propertyInfo.GetGetMethod()); //push on stack the property value

                if (propertyInfo.PropertyType.IsValueType)
                    il.Emit(OpCodes.Box, propertyInfo.PropertyType); // box the property value to correct type

                //push on stack the html format template (htmlAs or the given default)
                if (HtmlAsDict.TryGetValue(propertyInfo, out HtmlAs htmlAs))
                {
                    il.Emit(OpCodes.Ldstr, htmlAs.Template); //HtmlAs format
                }
                else
                {
                    il.Emit(OpCodes.Ldarg_2); //default format
                }

                il.Emit(OpCodes.Ldarg_3); //push on stack the formatting type (simple(false) or table entry(true))
                il.Emit(OpCodes.Call, formatterForHtml); //call formatter
                il.Emit(OpCodes.Call, concat); // concat with html from last property
            }
            il.Emit(OpCodes.Ret); // ret resulting html
        }

        //Returns html of object in list style
        public string ToHtml(object source)
        {
            Type objType = source.GetType();
            StringBuilder html = new StringBuilder("");
            if (HtmlFormatterTypes.ContainsKey(objType))
            {
                return html.AppendLine(ObjPropertiesToHtml(source,null, false)).ToString();
            }
            html.AppendLine("<ul class=\'list-group\'>\n");
            html.AppendLine(ObjPropertiesToHtml(source, "<li class=\'list-group-item\'><strong>{name}</strong>: {value}</li>", false));
            return html.AppendLine("</ul>").ToString();
        }
        
        //Returns html of object array in table style
        public string ToHtml<T>(IEnumerable<T> arr)
        {
            StringBuilder html = new StringBuilder("");
            if (HtmlFormatterTypes.ContainsKey(typeof(T)))
            {
                return html.AppendLine(ObjPropertiesToHtml(arr, null, false)).ToString();
            }
            html.AppendLine("<table class='table table-hover'>\n");

            //fill table header
            html.AppendLine("<thead>\n<tr>");
            
            PropertyInfo[] properties = GetPropertyInfoArray(typeof(T));
            HtmlAttributes_init(properties);
            
            foreach (var property in properties)
                if (!htmlIgnoreList.Contains(property))
                    html.AppendLine("<th>" + property.Name + "</th>");

            html.AppendLine("</tr>\n<thead>\n<tbody>");

            //fill table lines
            foreach (var obj in arr)
            {
                html.AppendLine("<tr>" + ObjPropertiesToHtml(obj, "{value}", true) + "</tr>");
            }
            
            return html.AppendLine("</tbody>\n</table>").ToString();
        }
	    
		//Returns custom HtmlEmit for type details html
	    public HtmlEmit ForTypeDetails<T>(Func<T, string> transf) {
		    Type type = typeof(T);
		    HtmlFormatterTypes.Add(type, new HtmlFormatObj<T>(transf));
		    return this;
	    }
	    //Returns custom HtmlEmit for type in table html for table headers and first table line //TODO
		public HtmlEmit ForTypeInTable<T>(IEnumerable<string> headers, Func<T, string> transf)
		{
		    Type type = typeof(T);
            HtmlFormatterTypes.Add(type, new HtmlFormatObj<T>(transf));
			return this;
		}
	    //Returns custom HtmlEmit for type list html //TODO
		public HtmlEmit ForSequenceOf<T>(Func<IEnumerable<T>, string> transf)
		{
		    Type type = typeof(T);
            HtmlFormatterTypes.Add(type, new HtmlFormatEnum<T>(transf));
			return this;
		}

	    class HtmlFormatObj<T> : IHtmlGetter { //TODO replace HtmlGetter with HtmlFormater and emit code to return the delegate func
		    private Func<T, string> formatter;

		    public HtmlFormatObj(Func<T, string> formatter_param) {
			    formatter = formatter_param;
		    }

	        public string GetHtml(object obj, string defaultTemplate, bool isTable)
	        {
	            return GetHtml(obj);
	        }

	        public string GetHtml(object obj) {
			    return formatter((T) obj);
		    }
	    }

        class HtmlFormatEnum<T> : IHtmlGetter
        {
            private Func<IEnumerable<T>, string> formatter;

            public HtmlFormatEnum(Func<IEnumerable<T>, string> formatter_param)
            {
                formatter = formatter_param;
            }

            public string GetHtml(object obj, string defaultTemplate, bool isTable)
            {
                return GetHtml(obj);
            }

            public string GetHtml(object obj)
            {
                return formatter((IEnumerable<T>) obj);
            }
        }
        

		//Gets html of object using an IHtmlGetter from dictionary or emits(and saves) a new one
		public string ObjPropertiesToHtml(object obj, string defaultTemplate, bool isTable)
        {
            IHtmlGetter htmlGetter;
            Type objType = obj.GetType();
            if (!HtmlFormatterTypes.TryGetValue(objType, out htmlGetter))
            {
                if (!htmlTypes.TryGetValue(objType, out htmlGetter))
                {
                    htmlGetter = EmitHtmlGetter(objType);
                    htmlTypes.Add(objType, htmlGetter);
                }
            }
            return htmlGetter.GetHtml(obj, defaultTemplate, isTable);
        }
    }
}