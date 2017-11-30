using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlReflect;
using HtmlReflectTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HtmlReflectTest
{
    [TestClass]
    public class TestEmitVsReflect
    {
        static HtmlectEmitTest htmlEmitTest = new HtmlectEmitTest();
        static HtmlectTest htmlReflectTest = new HtmlectTest();
        
        [TestMethod]
        public void TestEmitVsReflectObject()
        {
            NBench.Bench(htmlReflectTest.ToHtmlTest, "Html Reflect for a single Object");
            NBench.Bench(htmlEmitTest.ToHtmlEmitTest, "Html Emit for a single Object");
        }

        [TestMethod]
        public void TestEmitVsReflectArray()
        {
            NBench.Bench(htmlReflectTest.ToHtmlArrayTest, "Html Reflect for an Object Array");
            NBench.Bench(htmlEmitTest.ToHtmlEmitArrayTest, "Html Emit for an Object Array");
        }
    }
}
