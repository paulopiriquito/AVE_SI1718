using System;
using System.Net;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovHubDb;
using MovHubDb.Model;

namespace HtmlReflectTest
{
    [TestClass]
    public class MovHubDbTest
    {
        [TestMethod]
        public void SearchTest()
        {
            MovieSearchItem expected = new MovieSearchItem();
            expected.Id = 181808;
            expected.Title = "Star Wars: The Last Jedi";
            expected.ReleaseDate = "2017-12-13";
            MovieSearchItem[] actual = new TheMovieDbClient().Search("Star Wars", 1);
            Assert.AreEqual(expected.Id, actual[0].Id);
            Assert.AreEqual(expected.Title, actual[0].Title);
            Assert.AreEqual(expected.ReleaseDate, actual[0].ReleaseDate);
        }
        
        [TestMethod]
        public void MovieDetailsTest()
        {
            Movie expected = new Movie();
            expected.Id = 11;
            expected.OriginalTitle = "Star Wars";
            expected.TagLine = "A long time ago in a galaxy far, far away...";
            expected.Budget = 11000000;
            expected.Overview = "Princess Leia is captured and held hostage by the evil Imperial forces in their effort to take over the galactic Empire. Venturesome Luke Skywalker and dashing captain Han Solo team together with the loveable robot duo R2-D2 and C-3PO to rescue the beautiful princess and restore peace and justice in the Empire.";
            expected.ReleaseDate = "1977-05-25";
            expected.Revenue = 775398007;
            expected.PosterPath = "/btTdmkgIvOi0FFip1sPuZI2oQG6.jpg";
            Movie actual = new TheMovieDbClient().MovieDetails(11);
            Assert.AreEqual(expected.Id,actual.Id);
            Assert.AreEqual(expected.Budget,actual.Budget);
            Assert.AreEqual(expected.OriginalTitle,actual.OriginalTitle);
            Assert.AreEqual(expected.Overview,actual.Overview);
            Assert.AreEqual(expected.PosterPath,actual.PosterPath);
            Assert.AreEqual(expected.ReleaseDate,actual.ReleaseDate);
            Assert.AreEqual(expected.Revenue,actual.Revenue);
            Assert.AreEqual(expected.TagLine,actual.TagLine);
        }
        
        [TestMethod]
        public void MovieCreditsTest()
        {
            CreditsItem expected = new CreditsItem();
            expected.Character = "Luke Skywalker";
            expected.Department = null;
            expected.Id = 2;
            expected.Name = "Mark Hamill";
            CreditsItem[] actual = new TheMovieDbClient().MovieCredits(11);
            Assert.AreEqual(expected.Character,actual[0].Character);
            Assert.AreEqual(expected.Department,actual[0].Department);
            Assert.AreEqual(expected.Id,actual[0].Id);
            Assert.AreEqual(expected.Name,actual[0].Name);
        }
        
        [TestMethod]
        public void PersonDetailsTest()
        {
            Person expected = new Person();
            expected.Id = 2;
            expected.Name = "Mark Hamill";
            expected.Birthday = "1951-09-25";
            expected.Deathday = null;
            expected.PlaceOfBirth = "Concord, California, USA";
            expected.Biography = "Mark Richard Hamill (born September 25, 1951) is an American actor, voice artist, producer, director, and writer. Hamill is best known for his role as Luke Skywalker in the original Star Wars trilogy and also well known for voice-acting characters such as the Joker in various animated series, animated films and video games, beginning with Batman: The Animated Series, the Skeleton king in Super Robot Monkey Team Hyperforce Go!, Fire Lord Ozai in Avatar: The Last Airbender, Master Eraqus in Kingdom Hearts: Birth by Sleep, Skips in Regular Show, and Senator Stampington on Metalocalypse.";
            expected.ProfilePath = "/fk8OfdReNltKZqOk2TZgkofCUFq.jpg";
            Person actual = new TheMovieDbClient().PersonDetais(2);
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Biography, actual.Biography);
            Assert.AreEqual(expected.Birthday, actual.Birthday);
            Assert.AreEqual(expected.Deathday, actual.Deathday);
            Assert.AreEqual(expected.PlaceOfBirth, actual.PlaceOfBirth);
            Assert.AreEqual(expected.ProfilePath, actual.ProfilePath);
        }
        
        [TestMethod]
        public void PersonMoviesTest()
        {
            PersonCreditsItem expected = new PersonCreditsItem();
            expected.Character = "Luke Skywalker";
            expected.Id = 11;
            expected.Title = "Star Wars";
            expected.ReleaseDate = "1977-05-25";
            expected.VoteAverage = (float) 8.1;
            PersonCreditsItem[] actual = new TheMovieDbClient().PersonMovies(2);
            Assert.AreEqual(expected.Character, actual[0].Character);
            Assert.AreEqual(expected.Id, actual[0].Id);
            Assert.AreEqual(expected.Title, actual[0].Title);
            Assert.AreEqual(expected.ReleaseDate, actual[0].ReleaseDate);
            Assert.AreEqual(expected.VoteAverage, actual[0].VoteAverage);
        }
        
        [TestMethod]
        public void GetHttpResponseTest()
        {
            WebClient client = new WebClient { Encoding = Encoding.UTF8 };
            String actual = client.DownloadString(
                "https://api.themoviedb.org/3/search/movie?include_adult=false&api_key=d109c79ba0d619a3a74f33532eb3aa06&query=war%20games");
        }
    }
}