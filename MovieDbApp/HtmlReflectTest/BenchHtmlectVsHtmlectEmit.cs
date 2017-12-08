using System.Collections;
using HtmlReflect;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovHubDb;
using MovHubDb.Model;

namespace HtmlReflectTest
{
    [TestClass]
    public class BenchHtmlectVsHtmlectEmit
    {
        private static int PAGES_TO_BENCH = 10;
        private static TheMovieDbClient movieDb = new TheMovieDbClient();
        private static Htmlect htmlReflectTest = new Htmlect();
        private static HtmlectEmit htmlectEmitTest = new HtmlectEmit();
        private static MovieSearchItem[] sampleArray = movieDb.Search("world", 1);
        static BenchHtmlectVsHtmlectEmit()
        {
            for (int i = 0; i < PAGES_TO_BENCH; i++)
            {
                MovieSearchItem[] querryItems = movieDb.Search("world", 1);
                MovieSearchItem[] newSample = new MovieSearchItem[querryItems.Length+sampleArray.Length];
                querryItems.CopyTo(newSample,0);
                sampleArray.CopyTo(newSample, querryItems.Length);
                sampleArray = newSample;
            }
        }

        [TestMethod]
        public void BenchHtmlectVsHtmlectEmit_MovieSerachItem()
        {
            NBench.Bench(BenchHtmlect, "Htmlect (+reflection) for a MovieSearchItem[] (lenght ="+sampleArray.Length+")");
            NBench.Bench(BenchHtmlectEmit, "HtmlectEmit (with il code emition) for a MovieSearchItem[] (lenght =" + sampleArray.Length + ")");
        }

        private static void BenchHtmlect()
        {
            htmlReflectTest.ToHtml(sampleArray);
        }

        private static void BenchHtmlectEmit()
        {
            htmlectEmitTest.ToHtml(sampleArray);
        }
    }
}
