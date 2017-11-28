using System;

namespace HtmlReflect
{
    public class HtmlIgnoreAttribute : IHtmlAttribute
    {
        public HtmlIgnoreAttribute(Attribute attribute)
        {
        }

        public string GetHtml(String name, String value)
        {
            return "";
        }

        public string GetHtmlTableHeader(string name)
        {
            return "";
        }

        public string GetHtmlTableLine(string name, string value)
        {
            return "";
        }
    }
}