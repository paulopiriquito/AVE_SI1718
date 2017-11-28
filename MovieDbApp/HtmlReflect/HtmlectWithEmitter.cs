using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HtmlReflect
{
    class HtmlectWithEmitter
    {
        private static Dictionary<Type, PropertyInfo[]> typeToProperty = new Dictionary<Type, PropertyInfo[]>();
        private static Dictionary<PropertyInfo, IHtmlAttribute> htmlAsAttribute = new Dictionary<PropertyInfo, IHtmlAttribute>();
        private static Dictionary<PropertyInfo, IGetter> propertyValueGetters = new Dictionary<PropertyInfo, IGetter>();

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

        private static IGetter getPropertyValueGetter(PropertyInfo p, object obj)
        {
            IGetter valueGetter;
            //check dictionary to prevent use of reflection
            if (!propertyValueGetters.ContainsKey(p))
            {
                valueGetter = HtmlEmit.EmitGetter(p, obj);
                propertyValueGetters.Add(p, valueGetter);
            }
            else
            {
                valueGetter = propertyValueGetters[p];
            }
            return valueGetter;
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
                    string value = propertyValueGetters[property].GetValueAsString(obj);
                    if (value != null)
                    {
                        result += htmlAttribute.GetHtml(property.Name, value);
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
                foreach (var property in properties)
                {
                    IHtmlAttribute htmlAttribute;
                    if (htmlAsAttribute.TryGetValue(property, out htmlAttribute))
                    {
                        string value = propertyValueGetters[property].GetValueAsString(obj);
                        if (value != null)
                        {
                            result += htmlAttribute.GetHtmlTableLine(property.Name, value);
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
