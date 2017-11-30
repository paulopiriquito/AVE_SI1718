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
                    HtmlIgnore ignore = (HtmlIgnore) Attribute.GetCustomAttribute(property,typeof(HtmlIgnore));
                    if (ignore == null)
                    {
                        HtmlAsDict.Add(property, (HtmlAs) Attribute.GetCustomAttribute(property, typeof(HtmlAs)));
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
                    HtmlAs htmlAsAttribute;
                    if (HtmlAsDict.TryGetValue(property, out htmlAsAttribute))
                    {
                        var value = property.GetValue(obj);
                        if (value != null)
                        {
                            result.AppendLine(htmlAsAttribute.GetHtml(property.Name, property.GetValue(obj).ToString()));
                        }
                        else
                        {
                            result.AppendLine(htmlAsAttribute.GetHtml(property.Name, ""));
                        }
                    }
                }
            }
            return result.AppendLine("</ul>").ToString();
        }

        public string ToHtml(object[] array)
        {
            StringBuilder result = new StringBuilder("<table class='table table-hover'>\n<thead>\n<tr>\n");
            Type objType = array[0].GetType();
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
                        result.AppendLine(htmlAsAttribute.GetHtmlTableHeader(property.Name));
                    }
                }
            }
            result.AppendLine("</tr>\n<thead>\n<tbody>");

            //fill table lines
            foreach (var obj in array)
            {   
                result.AppendLine("<tr>");
                foreach(var property in properties)
                {
                    if (!HtmlIgnoreList.Contains(property))
                    {
                        HtmlAs htmlAsAttribute;
                        if (HtmlAsDict.TryGetValue(property, out htmlAsAttribute))
                        {
                            object value = property.GetValue(obj);
                            if (value != null)
                            {
                                result.AppendLine(htmlAsAttribute.GetHtmlTableLine(property.Name, property.GetValue(obj).ToString()));
                            }
                            else
                            {
                                result.AppendLine(htmlAsAttribute.GetHtmlTableLine(property.Name, ""));
                            }

                        }
                    }
                }
                result.AppendLine("</tr>");
            }
            return result.AppendLine("</tbody>\n</table>").ToString();
        }

    }
    
}
