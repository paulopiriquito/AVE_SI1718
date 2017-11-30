using System;

namespace HtmlReflect
{
    public class HtmlIgnore : IHtmlAttribute
    {
        public string ToString()
        {
            return "";
        }

        public override string GetHtml(string name, string value)
        {
            return "";
        }

        public override string GetHtmlTableHeader(string name)
        {
            return "";
        }

        public override string GetHtmlTableLine(string name, string value)
        {
            return "";
        }
    }
}