using System;

namespace HtmlReflect
{
    public interface IHtmlAttribute
    {
        String GetHtml(String name, String value);

        String GetHtmlTableHeader(String name);

        String GetHtmlTableLine(String name, String value);
    }
}