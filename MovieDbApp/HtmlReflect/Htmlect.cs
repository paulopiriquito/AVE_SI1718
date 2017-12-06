using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;


namespace HtmlReflect
{
    public class Htmlect
    {
        private static Dictionary<Type, PropertyInfo[]> TypeToProperty = new Dictionary<Type, PropertyInfo[]>();
        
        private static Dictionary<PropertyInfo, HtmlAs> HtmlAsDict = new Dictionary<PropertyInfo, HtmlAs>();
        private static List<PropertyInfo> HtmlIgnoreList = new List<PropertyInfo>();

        public static void HtmlAttributes_init(PropertyInfo[] properties)
        {
            foreach (var property in properties)
            {
                if (!HtmlIgnoreList.Contains(property) && !HtmlAsDict.ContainsKey(property))
                {
                    HtmlIgnore ignore = (HtmlIgnore) property.GetCustomAttribute(typeof(HtmlIgnore));
                    if (ignore == null)
                    {
                        HtmlAs htmlAs = (HtmlAs) property.GetCustomAttribute(typeof(HtmlAs));
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
            PropertyInfo[] properties;
            //check dictionary to prevent use of repeated reflection
            if (!TypeToProperty.TryGetValue(type, out properties))
            {
                properties = type.GetProperties();
                TypeToProperty.Add(type, properties);
            }
            return properties;
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
                    string name = property.Name;
                    var propertyvalue = property.GetValue(obj);
                    string value = propertyvalue == null ? "" : propertyvalue.ToString();

                    if (HtmlAsDict.TryGetValue(property, out var htmlAsAttribute))
                        result.AppendLine(htmlAsAttribute.Template.Replace("{name}", name).Replace("{value}", value));
                    else
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
                        result.AppendLine("<th>"+property.Name+"</th>");
                
                result.AppendLine("</tr>\n<thead>\n<tbody>");

                //fill table lines
                foreach (var obj in array)
                {
                    result.AppendLine("<tr>");
                    foreach (var property in properties)
                    {
                        if (!HtmlIgnoreList.Contains(property))
                        {
                            string name = property.Name;
                            var propertyvalue = property.GetValue(obj);
                            string value = propertyvalue == null ? "" : propertyvalue.ToString();
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
