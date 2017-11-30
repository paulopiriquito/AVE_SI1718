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
        string ToHtml(object target, PropertyInfo property, string format);
        string GetPropertyName(PropertyInfo property);
    }

    public class HtmlectEmit
    {
        private static Dictionary<Type, IHtmlGetter> gettersDictionary = new Dictionary<Type, IHtmlGetter>();
        private static Dictionary<Type, PropertyInfo[]> TypeToProperty = new Dictionary<Type, PropertyInfo[]>();
        private static Dictionary<PropertyInfo, HtmlAs> HtmlAsDict = new Dictionary<PropertyInfo, HtmlAs>();
        private static List<PropertyInfo> HtmlIgnoreList = new List<PropertyInfo>();

        public static void HtmlAttributes_init(PropertyInfo[] properties)
        {
            foreach (var property in properties)
            {
                if (!HtmlIgnoreList.Contains(property) && !HtmlAsDict.ContainsKey(property))
                {
                    HtmlIgnore ignore = (HtmlIgnore)Attribute.GetCustomAttribute(property, typeof(HtmlIgnore));
                    if (ignore == null)
                    {
                        HtmlAsDict.Add(property, (HtmlAs)Attribute.GetCustomAttribute(property, typeof(HtmlAs)));
                    }
                    else
                        HtmlIgnoreList.Add(property);
                }
            }
        }

        public static PropertyInfo[] GetPropertyInfoArray(Type type)
        {
            PropertyInfo[] properties;
            //check dictionary to prevent use of repeated reflection
            if (!TypeToProperty.ContainsKey(type))
            {
                properties = type.GetProperties();
                TypeToProperty.Add(type, properties);
            }
            else
            {
                properties = TypeToProperty[type];
            }
            return properties;
        }

        private static IHtmlGetter EmitGetter(Type klassType, PropertyInfo p)
        {
            AssemblyName assemblyName = new AssemblyName("DynamicHtmlGetter");
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name, assemblyName.Name + ".dll");

            TypeBuilder typeBuilder = moduleBuilder.DefineType("HtmlGetter", TypeAttributes.Public);
            typeBuilder.AddInterfaceImplementation(typeof(IHtmlGetter));

            MethodBuilder ToHtml = typeBuilder.DefineMethod("ToHtml", MethodAttributes.Public | MethodAttributes.Static, typeof(string), new Type[] { typeof(object), typeof(PropertyInfo), typeof(string)});
            MethodBuilder GetPropertyName = typeBuilder.DefineMethod("ToHtml", MethodAttributes.Public | MethodAttributes.Static, typeof(string), new Type[] {typeof(PropertyInfo)});
            ILGenerator ToHtmlObjIL = ToHtml.GetILGenerator();
            ILGenerator GetPropertyNameIL = GetPropertyName.GetILGenerator();

            //TODO GetPropertyNameIL

            //TODO ToHtmlObjIL Emit

            Type t = typeBuilder.CreateType();
            object getter = Activator.CreateInstance(t);
            return (IHtmlGetter) getter;
        }

        public string ToHtml(object source)
        {
            StringBuilder result = new StringBuilder("<ul class=\'list-group\'>\n");
            Type objType = source.GetType();
            PropertyInfo[] properties = GetPropertyInfoArray(objType);
            HtmlAttributes_init(properties);
            foreach (var property in properties)
            {
                if (!HtmlIgnoreList.Contains(property))
                {
                    HtmlAs htmlAsAttribute;
                    if (HtmlAsDict.TryGetValue(property, out htmlAsAttribute))
                    {
                        IHtmlGetter getter;
                        if (!gettersDictionary.TryGetValue(objType, out getter))
                        {
                            getter = EmitGetter(objType, property);
                            gettersDictionary.Add(objType, getter);
                        }
                        result.AppendLine(getter.ToHtml(source, property, htmlAsAttribute.Get_htmlAs()));
                    }
                }
            }
            return result.AppendLine("</ul>").ToString();
        }

        public string ToHtml(object[] sources)
        {
            StringBuilder result = new StringBuilder("<table class='table table-hover'>\n<thead>\n<tr>\n");
            Type objType = sources[0].GetType();
            PropertyInfo[] properties = GetPropertyInfoArray(objType);

            HtmlAttributes_init(properties);

            //fill table header
            foreach (var property in properties)
            {
                if (!HtmlIgnoreList.Contains(property))
                {
                    HtmlAs htmlAsAttribute;
                    if (HtmlAsDict.TryGetValue(property, out htmlAsAttribute))
                    {
                        result.AppendLine("<th>");
                        IHtmlGetter getter;
                        if (!gettersDictionary.TryGetValue(objType, out getter))
                        {
                            getter = EmitGetter(objType, property);
                            gettersDictionary.Add(objType, getter);
                        }
                        result.AppendLine(getter.GetPropertyName(property));
                        result.AppendLine("</th>");
                    }
                }
            }
            result.AppendLine("</tr>\n<thead>\n<tbody>");

            //fill table lines
            foreach (var obj in sources)
            {
                result.AppendLine("<tr>");
                foreach (var property in properties)
                {
                    if (!HtmlIgnoreList.Contains(property))
                    {
                        HtmlAs htmlAsAttribute;
                        if (HtmlAsDict.TryGetValue(property, out htmlAsAttribute))
                        {
                            result.AppendLine("<td>");
                            IHtmlGetter getter;
                            if (!gettersDictionary.TryGetValue(objType, out getter))
                            {
                                getter = EmitGetter(objType, property);
                                gettersDictionary.Add(objType, getter);
                            }
                            result.AppendLine(getter.ToHtml(obj, property, htmlAsAttribute.Get_htmlAs()));
                            result.AppendLine("</td>");
                        }
                    }
                }
                result.AppendLine("</tr>");
            }
            return result.AppendLine("</tbody>\n</table>").ToString(); ;
        }
    }
}
