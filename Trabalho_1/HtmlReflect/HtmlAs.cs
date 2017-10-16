using System;

namespace HtmlReflect
{
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class HtmlAs : System.Attribute
    {
        private String asString;

        public HtmlAs(String htmlAs)
        {
            asString = htmlAs;
        }

        public string AsString => asString;
    }
}