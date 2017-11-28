using System;
using HtmlReflect;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HtmlReflectTest
{
    public class CustomAttributeTest
    {
        private class TestObj
        {
            private string testingNonAttribute, testingAttribute;

            [HtmlAs("Success was {value}")]
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
        public class HtmlectTest
        {
            [TestMethod]
            public void ToHtmlTest()
            {
                TestObj obj = new TestObj();
                obj.TestingAsHtmlAttribute = "true";
                obj.TestingHtmlIgnoreAttribute = "This cannot show";
                Htmlect htmlect = new Htmlect();
                String expected = "<ul class=\'list-group\'>\nSuccess was true</ul>";
                String actual = htmlect.ToHtml(obj);
                Assert.AreEqual(expected, actual);
            }

            [TestMethod]
            public void ToHtmlArrayTest()
            {
                TestObj[] obj = new TestObj[2];
                obj[0] = new TestObj();
                obj[1] = new TestObj();
                obj[0].TestingAsHtmlAttribute = "true";
                obj[0].TestingHtmlIgnoreAttribute = "This cannot show";
                obj[1].TestingAsHtmlAttribute = "TRUE";
                obj[1].TestingHtmlIgnoreAttribute = "..This cannot show..";
                Htmlect htmlect = new Htmlect();
                String expected = "<table class='table table-hover'>\n<thead>\n<tr>\n<th>TestingAsHtmlAttribute</th></tr>\n<thead>\n<tbody>\n<tr><td>Success was true</td></tr>\n<tr><td>Success was TRUE</td></tr>\n</tbody>\n</table>";
                String actual = htmlect.ToHtml(obj);
                Assert.AreEqual(expected, actual);
            }
        }
    }
}