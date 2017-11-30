using System;

namespace HtmlReflect
{
    public interface IHtmlAttribute
    {
        string GetHtml(string name, string value);

        string GetHtmlTableHeader(string name);

        string GetHtmlTableLine(string name, string value);
    }
}