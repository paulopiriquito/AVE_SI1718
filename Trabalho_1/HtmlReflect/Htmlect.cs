using System;
using System.Collections.Generic;
using System.Reflection;

namespace HtmlReflect
{
    public class Htmlect
    {
        private static Dictionary<Type, PropertyInfo[]> typeToProperty = new Dictionary<Type, PropertyInfo[]>();
        
        public string ToHtml(object obj)
        {
            var result = "<ul class=\'list-group\'>\n";
            var objType = obj.GetType();
            PropertyInfo[] properties;
            if (!typeToProperty.ContainsKey(objType))
            {
                properties = objType.GetProperties();
                typeToProperty.Add(objType,properties);
            }
            else
            {
                properties = typeToProperty[objType];
            }
            foreach (var property in properties)
                {
                    if (!Attribute.IsDefined(property, typeof(HtmlIgnore)))
                    {
                        if (Attribute.IsDefined(property, typeof(HtmlAs)))
                        {

                            var format = (HtmlAs) Attribute.GetCustomAttribute(property, typeof(HtmlAs));
                            var html = format.AsString;
                            html = html.Replace("{name}", property.Name);
                            html = html.Replace("{value}", property.GetValue(obj).ToString());
                            result += html;
                        }
                        else
                        {
                            result += string.Format("<li class=\'list-group-item\'><strong>{0}</strong>:{1}</li>\n",
                                property.Name, " " + property.GetValue(obj));
                        }
                    }
                }
            return result += "</ul>";
        }

        public string ToHtml(object[] array)
        {
            Type objType = array[0].GetType();
            PropertyInfo[] properties;
            string result = "<table class='table table-hover'>\n<thead>\n<tr>\n";
            
            //check dictionary to prevent use of reflection
            if (!typeToProperty.ContainsKey(objType))
            {
                properties = objType.GetProperties();
                typeToProperty.Add(objType,properties);
            }
            else
            {
                properties = typeToProperty[objType];
            }
            
            //fill table header
            foreach (var property in properties)
            {
                if (!Attribute.IsDefined(property, typeof(HtmlIgnore)))
                {
                    result += "<th>" + property.Name + "</th>";
                }
            }
            result += "</tr>\n<thead>\n<tbody>\n";

            //fill table lines
            foreach (var obj in array)
            {   
                result += "<tr>";
                foreach(var property in properties)
                {
                    if (!Attribute.IsDefined(property, typeof(HtmlIgnore)))
                    {
                        if (Attribute.IsDefined(property, typeof(HtmlAs)))
                        {
                            var format = (HtmlAs) Attribute.GetCustomAttribute(property, typeof(HtmlAs));
                            String html = format.AsString;
                            html = html.Replace("{name}", property.Name);
                            html = html.Replace("{value}", property.GetValue(obj).ToString());
                            result += "<td>" + html+ "</td>";
                        }
                        else
                        {
                            result += "<td>" + property.GetValue(obj) + "</td>";
                        }
                    }
                }
                result += "</tr>\n";
            }
            return result += "</tbody>\n</table>";
        }
    }
}
