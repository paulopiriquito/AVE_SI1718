using System;

namespace HtmlReflect
{
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class HtmlAs : IHtmlAttribute
    {
        private string _htmlAs;
        
        public HtmlAs(string htmlFormat)
        {
            _htmlAs = htmlFormat;
        }


        public string Get_htmlAs()
        {
            if (_htmlAs == null)
            {
                return "<li class=\'list-group-item\'><strong>{name}</strong>: {value}</li>";
            }
            return _htmlAs;
        }

        public override string GetHtml(String name, String value)
        {
            if (_htmlAs != null)
            {
                string toReturn = _htmlAs.Replace("{name}", name);
                return toReturn.Replace("{value}", value);
            }
            else
            {
                string toReturn = string.Format("<li class=\'list-group-item\'><strong>{0}</strong>:{1}</li>",
                    name, " " + value);
                return toReturn;
            }
        }

        public override string GetHtmlTableHeader(string name)
        {
            return "<th>" + name + "</th>";
        }

        public override string GetHtmlTableLine(string name, string value)
        {
            if (_htmlAs != null)
            {
                String html = _htmlAs;
                html = html.Replace("{name}", name);
                html = html.Replace("{value}", value);
                return "<td>" + html + "</td>";
            }
            return "<td>" + value + "</td>";
        }
    }
}