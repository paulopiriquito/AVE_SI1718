﻿using System;
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

        //Inits HtmlIgnoreList and HtmlAsDict for a set of PropertyInfo to prevent repeated reflection on getting property attributes
        private static void HtmlAttributes_init(PropertyInfo[] properties)
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
        
        //Checks and updates dictionary to prevent use of repeated reflection on Type.GetProperties()
        private static PropertyInfo[] GetPropertyInfoArray(Type type)
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
            StringBuilder html = new StringBuilder("<ul class=\'list-group\'>\n");
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
                        html.AppendLine(htmlAsAttribute.Template.Replace("{name}", name).Replace("{value}", value));
                    else
                        html.AppendLine($"<li class=\'list-group-item\'><strong>{name}</strong>: {value}</li>");
                }
            }
            return html.AppendLine("</ul>").ToString();
        }

        public string ToHtml(object[] array)
        {
            StringBuilder html = new StringBuilder("<table class='table table-hover'>\n");

            Type objType = array.GetType().GetElementType();

            PropertyInfo[] properties = GetPropertyInfoArray(objType);
            HtmlAttributes_init(properties);

            //fill table header
            html.AppendLine("<thead>\n<tr>");

            foreach (var property in properties)
                if (!HtmlIgnoreList.Contains(property))
                    html.AppendLine("<th>" + property.Name + "</th>");

            html.AppendLine("</tr>\n<thead>\n<tbody>");

            //fill table lines
            foreach (var obj in array)
            {
                html.AppendLine("<tr>");
                foreach (var property in properties)
                {
                    if (!HtmlIgnoreList.Contains(property))
                    {
                        string name = property.Name;
                        var propertyvalue = property.GetValue(obj);
                        string value = propertyvalue == null ? "" : propertyvalue.ToString();
                        html.AppendLine("<td>");

                        if (HtmlAsDict.TryGetValue(property, out var htmlAsAttribute))
                            html.AppendLine(htmlAsAttribute.Template.Replace("{name}", name).Replace("{value}", value));
                        else
                            html.AppendLine(value);

                        html.AppendLine("</td>");
                    }
                }
                html.AppendLine("</tr>");
            }
            return html.AppendLine("</tbody>\n</table>").ToString();
        }
    }
}
