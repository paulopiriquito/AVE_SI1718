namespace HtmlReflect
{
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public abstract class IHtmlAttribute : System.Attribute
    {
        public abstract string GetHtml(string name, string value);

        public abstract string GetHtmlTableHeader(string name);

        public abstract string GetHtmlTableLine(string name, string value);
    }
}