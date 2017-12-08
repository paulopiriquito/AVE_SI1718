using System;
using System.Reflection;
using HtmlReflect;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HtmlReflectTest
{
    public class CustomAttributeTest
    {
        private class TestObj
        {
            private string testingNonAttribute, testingAttribute;

            [HtmlAs("{value}")]
            public string TestingAsHtmlAttribute
            {
                get => testingNonAttribute;
                set => testingNonAttribute = value;
            }
            
            [HtmlIgnore]
            public string TestingHtmlIgnoreAttribute
            {
                get => testingAttribute;
                set => testingAttribute = value;
            }
            
        }
        
        [TestClass]
        public class HtmlAttributeTest
        {
            private static TestObj obj = new TestObj();
            private static HtmlAs htmlAs =(HtmlAs) obj.GetType().GetProperty("TestingAsHtmlAttribute").GetCustomAttribute(typeof(HtmlAs));
            private static HtmlIgnore htmlIgnore = (HtmlIgnore)obj.GetType().GetProperty("TestingHtmlIgnoreAttribute").GetCustomAttribute(typeof(HtmlIgnore));
            [TestMethod]
            public void HtmlAttribute_Test()
            {
                Assert.AreEqual("{value}", htmlAs.Template);
                Assert.IsNotNull(htmlIgnore);
            }
        }
    }
}