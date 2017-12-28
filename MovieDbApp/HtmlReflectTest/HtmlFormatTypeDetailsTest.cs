using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text.RegularExpressions;
using HtmlReflect;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovHubDb;
using MovHubDb.Model;


namespace HtmlReflectTest
{
    [TestClass]
    public class HtmlFormatTypeDetailsTest
    {
        private static Movie movie;
        private static IEnumerable<string> headers = new string[] { "Budget", "Vote Average" };
        private static String expectedMovieHtml =
            "<ul class='list-group'><ul class='list-group'><li class='list-group-item'><strong>OriginalTitle</strong>:Star Wars</li></ul>";
        private static MovieSearchItem[] movies;
        private static String expectedMoviesTypeInTables =
            "<ul class='list-group'><tr><td>11000000</td><td>8,1</td></tr></ul>";
        private static HtmlEmit htmlectEmit = new HtmlEmit();
        static HtmlFormatTypeDetailsTest()
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
            string result = htmlectEmit.ToHtml(movie);
            result = Regex.Replace(result, @"\t|\n|\r", "");
            Assert.AreEqual(expectedMovieHtml, result);
        }

        [TestMethod]
        public void HtmlFormatterForTypeInTableTest()
        {
            htmlectEmit.ForTypeInTable<Movie>(headers, mov =>
            {
                const string template = "<tr><td>{0}</td><td>{1}</td></tr>";
                return String.Format(template, mov.Budget, mov.VoteAverage);
            });
            string result = htmlectEmit.ToHtml(movie);
            result = Regex.Replace(result, @"\t|\n|\r", "");
            Assert.AreEqual(expectedMoviesTypeInTables, result);
        }

        [TestMethod]
        public void HtmlFormatterForSequenceOfTest()
        {
            htmlectEmit.ForSequenceOf<MovieSearchItem>(movies =>
            {
                string movieLine = movies.Aggregate("", (prev, mov) => prev + "<li>" + mov.Id + "</li>");
                return "<h1>Movie ids</h1><ul>" + movieLine + "</h1>";
            });
            string result = htmlectEmit.ToHtml(movies);
            result = Regex.Replace(result, @"\t|\n|\r", "");
            Assert.AreEqual(null, result);
        }

    }
}
