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
    public class HtmlEmitFormatedTest
    {
        private static Movie movie;
        private static Person expected;
        private static IEnumerable<string> headers = new string[] { "Name", "Birthday" };
        private static string expectedMovieHtml =
            "<ul class=\'list-group\'><li class=\'list-group-item\'><strong>OriginalTitle</strong>:Star Wars</li>";
        private static MovieSearchItem[] movies;
        private static string expectedPersonTypeInTables =
            "<table class=\'table table-hover\'>< thead > < tr ><th>Name</th><th>Birthday</th></tr><thead><tbody><tr><td>Mark Hamill</td><td>1951-09-25</td></tr>";

        private static string expectedForSequenceOf = "<h1>Movie ids</h1><ul><li>11</li></h1>";
        private static HtmlEmit htmlectEmit = new HtmlEmit();
        static HtmlEmitFormatedTest()
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
            expected = new Person();
            expected.Id = 2;
            expected.Name = "Mark Hamill";
            expected.Birthday = "1951-09-25";
            expected.Deathday = null;
            expected.PlaceOfBirth = "Concord, California, USA";
            expected.Biography = "From Wikipedia, the free encyclopedia.\n\nMark Richard Hamill (born September 25, 1951) is an American actor, voice artist, producer, director, and writer. Hamill is best known for his role as Luke Skywalker in the original Star Wars trilogy and also well known for voice-acting characters such as the Joker in various animated series, animated films and video games, beginning with Batman: The Animated Series, the Skeleton king in Super Robot Monkey Team Hyperforce Go!, Fire Lord Ozai in Avatar: The Last Airbender, Master Eraqus in Kingdom Hearts: Birth by Sleep, Skips in Regular Show, and Senator Stampington on Metalocalypse.\n\nDescription above from the Wikipedia article Mark Hamill, licensed under CC-BY-SA, full list of contributors on Wikipedia .";
            expected.ProfilePath = "/ws544EgE5POxGJqq9LUfhnDrHtV.jpg";
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
            htmlectEmit.ForTypeInTable<Person>(headers, per =>
            {
                const string template = "<tr><td>{0}</td><td>{1}</td></tr>";
                return String.Format(template, per.Name, per.Birthday);
            });
            string result = htmlectEmit.ToHtml(expected);
            result = Regex.Replace(result, @"\t|\n|\r", "");
            Assert.AreEqual(expectedPersonTypeInTables, result);
        }

        [TestMethod]
        public void HtmlFormatterForSequenceOfTest()
        {
            HtmlEmit htmlEmit = htmlectEmit.ForSequenceOf<MovieSearchItem>(movies =>
            {
                string movieLine = movies.Aggregate("", (prev, mov) => prev + "<li>" + mov.Id + "</li>");
                return "<h1>Movie ids</h1><ul>" + movieLine + "</h1>";
            });
            string result = htmlEmit.ToHtml(movies);
            result = Regex.Replace(result, @"\t|\n|\r", "");
            Assert.AreEqual(expectedForSequenceOf, result);
        }

    }
}
