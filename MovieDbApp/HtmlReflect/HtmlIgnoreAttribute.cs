using System;

namespace HtmlReflect
{
    public class HtmlIgnoreAttribute : IHtmlAttribute
    {
        public string ToString()
        {
            return "";
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