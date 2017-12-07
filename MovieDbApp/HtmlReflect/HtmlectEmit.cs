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
        string GetHtml(object obj);
    }
    public abstract class HtmlGetter : IHtmlGetter
    {
        public static string AsObject(object value)
        {
            return value == null ? "" : value.ToString();
        }
        public static string AsList(object[] array)
        {
            string str = "";
            foreach (var obj in array)
            {
                str += "<tr>" + HtmlectEmit.ObjPropertiesToHtml(obj) + "</tr>";
            }
            return str;
        }
        public abstract string GetHtml(object obj);
    }
    public class HtmlectEmit
    {
        static readonly MethodInfo formatterForArray = typeof(HtmlGetter).GetMethod("AsList", new Type[] {typeof(object[]) });
        static readonly MethodInfo formatterForObject = typeof(HtmlGetter).GetMethod("AsObject", new Type[] {typeof(object) });

        static readonly MethodInfo concat = typeof(String).GetMethod("Concat", new Type[] { typeof(string), typeof(string) });
        static readonly MethodInfo replace = typeof(String).GetMethod("Replace", new Type[] {typeof(string), typeof(string) });
        static readonly MethodInfo toString = typeof(object).GetMethod("ToString", new Type[] {});

        private static Dictionary<Type, IHtmlGetter> htmlTypes = new Dictionary<Type, IHtmlGetter>();
        private static Dictionary<Type, PropertyInfo[]> TypeProperties = new Dictionary<Type, PropertyInfo[]>();
        private static Dictionary<PropertyInfo, HtmlAs> HtmlAsDict = new Dictionary<PropertyInfo, HtmlAs>();
        private static List<PropertyInfo> HtmlIgnoreList = new List<PropertyInfo>();
        
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
        public static PropertyInfo[] GetPropertyInfoArray(Type type)
        {
            //check dictionary to prevent use of repeated reflection
            if (!TypeProperties.TryGetValue(type, out var properties))
            {
                properties = type.GetProperties();
                TypeProperties.Add(type, properties);
            }
            return properties;
        }
        public static IHtmlGetter EmitHtmlGetter(Type klassType)
        {
            AssemblyName assemblyName = new AssemblyName(klassType.Name + "DynamicHtmlEmiter");
            AssemblyBuilder assemblyBuilder =
                AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder moduleBuilder =
                assemblyBuilder.DefineDynamicModule(assemblyName.Name, assemblyName.Name + ".dll");
            TypeBuilder typeBuilder = moduleBuilder.DefineType(klassType.Name + "HtmlGetter", TypeAttributes.Public, typeof(HtmlGetter));

            MethodBuilder getHtmlMethodBuilder = typeBuilder.DefineMethod("GetHtml", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.ReuseSlot,
                typeof(string),
                new Type[] { typeof(object) }
            );

            ILGenerator getHtmlGenerator = getHtmlMethodBuilder.GetILGenerator();
            bool isArray = false;
            Type thisType = klassType;
            if (klassType.IsArray)
            {
                thisType = klassType.GetElementType();
                isArray = true;
            }

            PropertyInfo[] properties = GetPropertyInfoArray(thisType);
            HtmlAttributes_init(properties);

            LocalBuilder localType = getHtmlGenerator.DeclareLocal(thisType);
            getHtmlGenerator.Emit(OpCodes.Ldarg_1);
            getHtmlGenerator.Emit(OpCodes.Castclass, thisType);
            getHtmlGenerator.Emit(OpCodes.Stloc_0, localType);
            
            
            getHtmlGenerator.Emit(OpCodes.Ldstr, "");

            foreach (var property in properties)
            {
                if (!HtmlIgnoreList.Contains(property))
                {
                    if (isArray)
                    {
                        getHtmlGenerator.Emit(OpCodes.Ldstr, "<td>");
                        getHtmlGenerator.Emit(OpCodes.Call, concat);
                        if (HtmlAsDict.TryGetValue(property, out var htmlAs))
                        {
                            getHtmlGenerator.Emit(OpCodes.Ldstr, htmlAs.Template);
                            getHtmlGenerator.Emit(OpCodes.Ldstr, "{name}");
                            getHtmlGenerator.Emit(OpCodes.Ldstr, property.Name);
                            getHtmlGenerator.Emit(OpCodes.Call, replace);
                            getHtmlGenerator.Emit(OpCodes.Ldstr, "{value}");
                            getHtmlGenerator.Emit(OpCodes.Ldloc, localType);
                            getHtmlGenerator.Emit(OpCodes.Call, property.GetGetMethod());
                            if (property.PropertyType.IsValueType)
                                getHtmlGenerator.Emit(OpCodes.Box, property.PropertyType);
                            getHtmlGenerator.Emit(OpCodes.Call, replace);
                            getHtmlGenerator.Emit(OpCodes.Call, formatterForArray);
                            getHtmlGenerator.Emit(OpCodes.Call, concat);
                        }
                        else
                        {
                            getHtmlGenerator.Emit(OpCodes.Ldloc, localType);
                            getHtmlGenerator.Emit(OpCodes.Call, property.GetGetMethod());
                            if (property.PropertyType.IsValueType)
                                getHtmlGenerator.Emit(OpCodes.Box, property.PropertyType);
                            getHtmlGenerator.Emit(OpCodes.Call, formatterForArray);
                            getHtmlGenerator.Emit(OpCodes.Call, concat);
                        }
                        //IL para array
                        getHtmlGenerator.Emit(OpCodes.Ldstr, "</td>");
                        getHtmlGenerator.Emit(OpCodes.Call, concat);
                    }
                    else
                    {
                        if (HtmlAsDict.TryGetValue(property, out var htmlAs))
                        {
                            getHtmlGenerator.Emit(OpCodes.Ldstr, htmlAs.Template);
                        }
                        else
                        {
                            getHtmlGenerator.Emit(OpCodes.Ldstr, "<li class=\'list-group-item\'><strong>{name}</strong>: {value}</li>");
                        }
                        getHtmlGenerator.Emit(OpCodes.Ldstr, "{name}");
                        getHtmlGenerator.Emit(OpCodes.Ldstr, property.Name);
                        getHtmlGenerator.Emit(OpCodes.Call, replace);
                        getHtmlGenerator.Emit(OpCodes.Ldstr, "{value}");
                        getHtmlGenerator.Emit(OpCodes.Ldloc, localType);
                        getHtmlGenerator.Emit(OpCodes.Call, property.GetGetMethod());
                        if (property.PropertyType.IsValueType)
                            getHtmlGenerator.Emit(OpCodes.Box, property.PropertyType);
                        getHtmlGenerator.Emit(OpCodes.Call, toString);
                        getHtmlGenerator.Emit(OpCodes.Call, replace);
                        getHtmlGenerator.Emit(OpCodes.Call, formatterForObject);
                        getHtmlGenerator.Emit(OpCodes.Call, concat);
                    }
                }
            }
            getHtmlGenerator.Emit(OpCodes.Ret);

            Type t = typeBuilder.CreateType();
            assemblyBuilder.Save(assemblyName.Name + ".dll");
            return (IHtmlGetter)Activator.CreateInstance(t);
        }
        public string ToHtml(object source)
        {
            StringBuilder html = new StringBuilder("<ul class=\'list-group\'>\n");
            html.AppendLine(ObjPropertiesToHtml(source));
            return html.AppendLine("</ul>").ToString();
        }
        public string ToHtml(object[] sources)
        {
            StringBuilder html = new StringBuilder("<table class='table table-hover'>\n");

            //fill table header
            html.AppendLine("<thead>\n<tr>");
            
            PropertyInfo[] properties = GetPropertyInfoArray(sources[0].GetType());
            foreach (var property in properties)
                if (!HtmlIgnoreList.Contains(property))
                    html.AppendLine("<th>" + property.Name + "</th>");

            html.AppendLine("</tr>\n<thead>\n<tbody>");

            //fill table lines
            html.AppendLine(ObjPropertiesToHtml(sources));

            return html.AppendLine("</tbody>\n</table>").ToString();
        }

        public static string ObjPropertiesToHtml(object obj)
        {
            IHtmlGetter htmlGetter;
            Type objType = obj.GetType();
            if (!htmlTypes.TryGetValue(objType, out htmlGetter))
            {
                htmlGetter = EmitHtmlGetter(objType);
                htmlTypes.Add(objType, htmlGetter);
            }
            return htmlGetter.GetHtml(obj);
        }
    }
}
