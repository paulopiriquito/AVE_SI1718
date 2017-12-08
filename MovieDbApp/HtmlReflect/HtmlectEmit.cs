using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace HtmlReflect
{
    public interface IHtmlGetter
    {
        string GetHtml(object obj, string defaultTemplate, bool isTable);
    }
    public abstract class HtmlGetter : IHtmlGetter
    {
        public static string FormatHtml(string name, object val, string format, bool isTable)
        {
            var value = val == null ? "" : val.ToString();
            string ret = format.Replace("{name}", name).Replace("{value}", value);
            if (isTable)
                return "<td>" + ret + "</td>";
            return ret;
        }
        public abstract string GetHtml(object target, string defaultTemplate, bool isTable);
    }
    public class HtmlectEmit
    {
        static readonly MethodInfo formatterForHtml = typeof(HtmlGetter).GetMethod("FormatHtml", new Type[] { typeof(String), typeof(object), typeof(String), typeof(bool) });
        static readonly MethodInfo concat = typeof(String).GetMethod("Concat", new Type[] { typeof(string), typeof(string) });

        private static Dictionary<Type, IHtmlGetter> htmlTypes = new Dictionary<Type, IHtmlGetter>(); //saves an IHtmlGetter for a Type
        private static Dictionary<Type, PropertyInfo[]> TypeProperties = new Dictionary<Type, PropertyInfo[]>(); //saves a PropertyInfo[] for a Type
        private static Dictionary<PropertyInfo, HtmlAs> HtmlAsDict = new Dictionary<PropertyInfo, HtmlAs>(); // saves property HtmlAs attributes
        private static List<PropertyInfo> HtmlIgnoreList = new List<PropertyInfo>(); // saves property HtmlIgnore attributes

        //inits HtmlIgnoreList and HtmlAsDict for a set of propertyinfos to prevent repeated reflection on getting property attributes
        public static void HtmlAttributes_init(PropertyInfo[] properties)
        {
            foreach (var property in properties)
            {
                if (!HtmlIgnoreList.Contains(property) && !HtmlAsDict.ContainsKey(property))
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
                        HtmlIgnoreList.Add(property);
                }
            }
        }
        //checks and updates dictionary to prevent use of repeated reflection on Type.GetProperties()
        public static PropertyInfo[] GetPropertyInfoArray(Type type)
        {
            if (!TypeProperties.TryGetValue(type, out var properties))
            {
                properties = type.GetProperties();
                TypeProperties.Add(type, properties);
            }
            return properties;
        }
        public static IHtmlGetter EmitHtmlGetter(Type klass)
        {
            AssemblyName assemblyName = new AssemblyName(klass.Name + "DynamicHtmlEmiter");
            AssemblyBuilder assemblyBuilder =
                AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder moduleBuilder =
                assemblyBuilder.DefineDynamicModule(assemblyName.Name, assemblyName.Name + ".dll");
            TypeBuilder typeBuilder = moduleBuilder.DefineType(klass.Name + "HtmlGetter", TypeAttributes.Public, typeof(HtmlGetter));

            MethodBuilder getHtmlMethodBuilder = typeBuilder.DefineMethod("GetHtml", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.ReuseSlot,
                typeof(string),
                new Type[] { typeof(object), typeof(string), typeof(bool) }
            );
            
            ILGenerator il = getHtmlMethodBuilder.GetILGenerator();

            PropertyInfo[] piInfos = GetPropertyInfoArray(klass);
            HtmlAttributes_init(piInfos);

            LocalBuilder target = il.DeclareLocal(klass);
            il.Emit(OpCodes.Ldarg_1);          // push target
            il.Emit(OpCodes.Castclass, klass); // castclass
            il.Emit(OpCodes.Stloc, target);    // store on local variable 


            il.Emit(OpCodes.Ldstr, "");

            foreach (PropertyInfo p in piInfos)
            {
                if (HtmlIgnoreList.Contains(p)) continue;

                il.Emit(OpCodes.Ldstr, p.Name);    // push on stack the property name
                il.Emit(OpCodes.Ldloc, target);    // ldloc target
                il.Emit(OpCodes.Call, p.GetGetMethod()); //push on stack the property value

                if (p.PropertyType.IsValueType)
                    il.Emit(OpCodes.Box, p.PropertyType); // box the property value to correct type
                
                //push on stack the html format template (htmlAs or the given default)
                if (HtmlAsDict.TryGetValue(p, out HtmlAs htmlAs))
                {
                    il.Emit(OpCodes.Ldstr, htmlAs.Template); //HtmlAs
                }
                else
                {
                    il.Emit(OpCodes.Ldarg_2); //default
                }
                
                il.Emit(OpCodes.Ldarg_3); //push on stack the formatting type (simple(false) or table entry(true))
                il.Emit(OpCodes.Call, formatterForHtml); //call formatter
                il.Emit(OpCodes.Call, concat); // concat with html from last property
            }
            il.Emit(OpCodes.Ret); // ret resulting html

            Type t = typeBuilder.CreateType();
            assemblyBuilder.Save(assemblyName.Name + ".dll");
            return (IHtmlGetter)Activator.CreateInstance(t);
        }
        public string ToHtml(object source)
        {
            StringBuilder html = new StringBuilder("<ul class=\'list-group\'>\n");
            html.AppendLine(ObjPropertiesToHtml(source, "<li class=\'list-group-item\'><strong>{name}</strong>: {value}</li>", false));
            return html.AppendLine("</ul>").ToString();
        }
        public string ToHtml(object[] sources)
        {
            StringBuilder html = new StringBuilder("<table class='table table-hover'>\n");

            //fill table header
            html.AppendLine("<thead>\n<tr>");
            
            PropertyInfo[] properties = GetPropertyInfoArray(sources.GetType().GetElementType());
            HtmlAttributes_init(properties);
            
            foreach (var property in properties)
                if (!HtmlIgnoreList.Contains(property))
                    html.AppendLine("<th>" + property.Name + "</th>");

            html.AppendLine("</tr>\n<thead>\n<tbody>");

            //fill table lines
            foreach (var obj in sources)
            {
                html.AppendLine("<tr>" + ObjPropertiesToHtml(obj, "{value}", true) + "</tr>");
            }
            
            return html.AppendLine("</tbody>\n</table>").ToString();
        }

        public static string ObjPropertiesToHtml(object obj, string defaultTemplate, bool isTable)
        {
            IHtmlGetter htmlGetter;
            Type objType = obj.GetType();
            if (!htmlTypes.TryGetValue(objType, out htmlGetter))
            {
                htmlGetter = EmitHtmlGetter(objType);
                htmlTypes.Add(objType, htmlGetter);
            }
            return htmlGetter.GetHtml(obj, defaultTemplate, isTable);
        }
    }
}
