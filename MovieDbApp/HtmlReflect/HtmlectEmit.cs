using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace HtmlReflect
{
    public interface IGetter
    {
        object GetValue(object target, PropertyInfo property);
    }

    public abstract class ValueGetter : IGetter
    {
        public abstract object GetValue(object target, PropertyInfo property);
    }

    public class HtmlectEmit
    {
        private static Dictionary<PropertyInfo, IGetter> gettersDictionary = new Dictionary<PropertyInfo, IGetter>();
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

        private static IGetter EmitGetter(Type klass, PropertyInfo property)
        {
            AssemblyName assemblyName = new AssemblyName("DynamicHtmlGetter");
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name, assemblyName.Name + ".dll");

            TypeBuilder typeBuilder = moduleBuilder.DefineType(klass.Name + "ValueGetter", TypeAttributes.Public, typeof(ValueGetter));

            MethodBuilder GetValueMethod = typeBuilder.DefineMethod("GetValue", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.ReuseSlot, typeof(object), new Type[] { typeof(object), typeof(PropertyInfo)});
            ILGenerator GetValueMethodIL = GetValueMethod.GetILGenerator();

            MethodInfo GetMethod = property.GetMethod;

            //TODO GetValueIL
            GetValueMethodIL.Emit(OpCodes.Ldarg_1);
            GetValueMethodIL.Emit(OpCodes.Castclass, klass);
            GetValueMethodIL.Emit(OpCodes.Callvirt, GetMethod);
            GetValueMethodIL.Emit(OpCodes.Box, property.PropertyType);
            GetValueMethodIL.Emit(OpCodes.Ret);
            
            
            Type t = typeBuilder.CreateType();
            assemblyBuilder.Save(assemblyName.Name + ".dll");
            object getter = Activator.CreateInstance(t);
            return (IGetter) getter;
        }

        public string ToHtml(object obj)
        {
            StringBuilder result = new StringBuilder("<ul class=\'list-group\'>\n");
            var objType = obj.GetType();
            PropertyInfo[] properties = GetPropertyInfoArray(objType);

            HtmlAttributes_init(properties);

            foreach (PropertyInfo property in properties)
            {
                if (!HtmlIgnoreList.Contains(property))
                {
                    if (!gettersDictionary.TryGetValue(property, out var getter))
                    {
                        getter = EmitGetter(objType, property);
                        gettersDictionary.Add(property, getter);
                    }
                    string name = property.Name;
                    string xPected = property.GetValue(obj).ToString();
                    var pvalue = getter.GetValue(obj, property);
                    string value = pvalue == null ? "" : pvalue.ToString();

                    if (HtmlAsDict.TryGetValue(property, out var htmlAsAttribute))
                        result.AppendLine($"<li class=\'list-group-item\'><strong>{name}</strong>: {value}</li>");
                }
            }
            return result.AppendLine("</ul>").ToString();
        }

        public string ToHtml(object[] array)
        {
            if (array != null)
            {
                StringBuilder result = new StringBuilder("<table class='table table-hover'>\n");

                Type objType = array[0].GetType();
                PropertyInfo[] properties = GetPropertyInfoArray(objType);

                HtmlAttributes_init(properties);

                //fill table header
                result.AppendLine("<thead>\n<tr>");

                foreach (var property in properties)
                    if (!HtmlIgnoreList.Contains(property))
                        result.AppendLine("<th>" + property.Name + "</th>");

                result.AppendLine("</tr>\n<thead>\n<tbody>");

                //fill table body
                foreach (var obj in array)
                {
                    result.AppendLine("<tr>");
                    foreach (var property in properties)
                    {
                        if (!HtmlIgnoreList.Contains(property))
                        {
                            string name = property.Name;
                            if (!gettersDictionary.TryGetValue(property, out var getter))
                            {
                                getter = EmitGetter(objType, property);
                                gettersDictionary.Add(property, getter);
                            }
                            var pvalue = getter.GetValue(obj, property);
                            string value = pvalue == null ? "" : pvalue.ToString();
                            
                            result.AppendLine("<td>");
                            if (HtmlAsDict.TryGetValue(property, out var htmlAsAttribute))
                                result.AppendLine(htmlAsAttribute.Template.Replace("{name}", name).Replace("{value}", value));
                            else
                                result.AppendLine(value);

                            result.AppendLine("</td>");
                        }
                    }
                    result.AppendLine("</tr>");
                }
                return result.AppendLine("</tbody>\n</table>").ToString();
            }
            return ""; //TODO Procura sem resultados
        }
    }
}