using System;

namespace HtmlReflect
{
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class HtmlAs : System.Attribute
    {
        private string template;

        public string Template
        {
            get => template;
        }

        public HtmlAs(string template)
        {
            this.template = template;
        }
    }
}