using System;

namespace HtmlReflect
{
    public class HtmlAsAttribute : IHtmlAttribute
    {
        private HtmlAs _htmlAs;
        
        public HtmlAsAttribute(Attribute attribute)
        {
            _htmlAs = (HtmlAs) attribute;
        }


        public string GetHtml(String name, String value)
        {
            if (_htmlAs != null)
            {
                string toReturn = _htmlAs.AsString.Replace("{name}", name);
                return toReturn.Replace("{value}", value);
            }
            else
            {
                string toReturn = string.Format("<li class=\'list-group-item\'><strong>{0}</strong>:{1}</li>\n",
                    name, " " + value);
                return toReturn;
            }
        }

        public string GetHtmlTableHeader(string name)
        {
            return "<th>" + name + "</th>";
        }

        public string GetHtmlTableLine(string name, string value)
        {
            if (_htmlAs != null)
            {
                String html = _htmlAs.AsString;
                html = html.Replace("{name}", name);
                html = html.Replace("{value}", value);
                return "<td>" + html + "</td>";
            }
            else
            {
                return "<td>" + value + "</td>";
            }
        }
    }
}