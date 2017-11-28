using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


namespace HtmlReflect
{
    public class Htmlect
    {
        private static Dictionary<Type, PropertyInfo[]> typeToProperty = new Dictionary<Type, PropertyInfo[]>();
        private static Dictionary<PropertyInfo, IHtmlAttribute> htmlAsAttribute = new Dictionary<PropertyInfo, IHtmlAttribute>();

        private static void init_HTML_Attributes(PropertyInfo[] properties)
        {
            foreach (var property in properties)
            {
                if (!htmlAsAttribute.ContainsKey(property))
                {
                    Attribute attribute = Attribute.GetCustomAttribute(property, typeof(HtmlIgnore));
                    if (attribute != null)
                    {
                        htmlAsAttribute.Add(property, new HtmlIgnoreAttribute(attribute));
                    }
                    else
                    {
                        attribute = Attribute.GetCustomAttribute(property, typeof(HtmlAs));
                        htmlAsAttribute.Add(property, new HtmlAsAttribute(attribute));
                    }
                }
            }
        }

        private static PropertyInfo[] getPropertyInfoArray(Type type)
        {
            PropertyInfo[] properties;
            //check dictionary to prevent use of reflection
            if (!typeToProperty.ContainsKey(type))
            {
                properties = type.GetProperties();
                typeToProperty.Add(type, properties);
            }
            else
            {
                properties = typeToProperty[type];
            }
            return properties;
        }

        public string ToHtml(object obj)
        {
            var result = "<ul class=\'list-group\'>\n";
            var objType = obj.GetType();
            PropertyInfo[] properties = getPropertyInfoArray(objType);

            init_HTML_Attributes(properties);
            
            foreach (PropertyInfo property in properties)
            {
                IHtmlAttribute htmlAttribute;
                if (htmlAsAttribute.TryGetValue(property, out htmlAttribute))
                {
                    object value = property.GetValue(obj);
                    if (value != null)
                    {
                        result += htmlAttribute.GetHtml(property.Name, property.GetValue(obj).ToString());
                    }
                    else
                    {
                        result += htmlAttribute.GetHtml(property.Name, "");
                    }
                }
            }
            return result += "</ul>";
        }

        public string ToHtml(object[] array)
        {
            string result = "<table class='table table-hover'>\n<thead>\n<tr>\n";
            Type objType = array[0].GetType();
            PropertyInfo[] properties = getPropertyInfoArray(objType);
            
            init_HTML_Attributes(properties);

            //fill table header
            foreach (var property in properties)
            {
                IHtmlAttribute htmlAttribute;
                if (htmlAsAttribute.TryGetValue(property, out htmlAttribute))
                {
                    result += htmlAttribute.GetHtmlTableHeader(property.Name);
                }
            }
            result += "</tr>\n<thead>\n<tbody>\n";

            //fill table lines
            foreach (var obj in array)
            {   
                result += "<tr>";
                foreach(var property in properties)
                {
                    IHtmlAttribute htmlAttribute;
                    if (htmlAsAttribute.TryGetValue(property, out htmlAttribute))
                    {
                        object value = property.GetValue(obj);
                        if (value != null)
                        {
                            result += htmlAttribute.GetHtmlTableLine(property.Name, property.GetValue(obj).ToString());
                        }
                        else
                        {
                            result += htmlAttribute.GetHtmlTableLine(property.Name, "");
                        }
                        
                    }
                }
                result += "</tr>\n";
            }
            return result += "</tbody>\n</table>";
        }

    }
    
}
