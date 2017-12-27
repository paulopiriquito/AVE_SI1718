using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using HtmlReflect;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovHubDb;
using MovHubDb.Model;


namespace HtmlReflectTest
{
    [TestClass]
    class HtmlFormatTest
    {
            private static Movie movie;
            private static String expectedMovieHtml =
            "<ul class='list-group'>\n<li class='list-group-item'><strong>OriginalTitle</strong>: Star Wars</li>";
            private static MovieSearchItem[] movies;
            private static String expectedMoviesHtml = "<table class='table table-hover'>\n<thead>\n<tr>\r\n<th>Id</th>\r\n<th>Title</th>\r\n<th>ReleaseDate</th>\r\n</tr>\n<thead>\n<tbody>\r\n<tr><td><a href='/movies/11'> 11 </a></td><td>Star Wars</td><td>1977-05-25</td></tr>\r\n</tbody>\n</table>\r\n";
            private static HtmlEmit htmlectEmit = new HtmlEmit();
            static HtmlFormatTest()
            {
                movie = new Movie();
                movie.Id = 11;
                movie.Budget = 11000000;
                movie.OriginalTitle = "Star Wars";
                movie.TagLine = "A long time ago in a galaxy far, far away...";
                movie.Overview = "Princess Leia is captured and held hostage by the evil Imperial forces in their effort to take over the galactic Empire. Venturesome Luke Skywalker and dashing captain Han Solo team together with the loveable robot duo R2-D2 and C-3PO to rescue the beautiful princess and restore peace and justice in the Empire.";
                movie.VoteAverage = (float)8.1;
                movie.PosterPath = "/btTdmkgIvOi0FFip1sPuZI2oQG6.jpg";
                movie.ReleaseDate = "1977-05-25";
                movies = new MovieSearchItem[1];
                MovieSearchItem item = new MovieSearchItem();
                item.Id = 11;
                item.ReleaseDate = "1977-05-25";
                item.Title = "Star Wars";
                item.VoteAverage = (float)8.1;
                movies[0] = item;
            }

            [TestMethod]
            public void HtmlFormatterDetailsTest()
            {
                htmlectEmit.ForTypeDetails<Movie>(mov =>
                    "<ul class='list-group'>\n<li class='list-group-item'><strong>OriginalTitle</strong>:" +
                    mov.OriginalTitle + "</li>");
                Assert.AreEqual(htmlectEmit.ToHtml(movie), expectedMovieHtml);
            }

            /*[TestMethod]
            public void ToHtmlEmitArrayTest()
            {
                Assert.AreEqual(htmlectEmit.ToHtml(movies), expectedMoviesHtml);
            }*/
    }
}
