﻿using System;
using HtmlReflect;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovHubDb;
using MovHubDb.Model;

namespace HtmlReflectTest
{
    [TestClass]
    public class HtmlectEmitTest
    {
        private static Movie movie;
        private static String expectedMovieHtml = "<ul class='list-group'>\n<li class='list-group-item'><strong>OriginalTitle</strong>: Star Wars</li><li class='list-group-item'><strong>TagLine</strong>: A long time ago in a galaxy far, far away...</li><li class='list-group-item'><a href='/movies/11/credits'>Cast and crew </a></li><li class='list-group-item'><strong>Budget</strong>: 11000000</li><li class='list-group-item'><strong>VoteAverage</strong>: 8,1</li><li class='list-group-item'><strong>ReleaseDate</strong>: 1977-05-25</li><div class='card-body bg-light'><div><strong>Overview</strong>:</div>Princess Leia is captured and held hostage by the evil Imperial forces in their effort to take over the galactic Empire. Venturesome Luke Skywalker and dashing captain Han Solo team together with the loveable robot duo R2-D2 and C-3PO to rescue the beautiful princess and restore peace and justice in the Empire.</div><div style=\"position:absolute; top:0; right:0;\"><img width=\"50%\" src=\"http://image.tmdb.org/t/p/w185//btTdmkgIvOi0FFip1sPuZI2oQG6.jpg\"></div>\r\n</ul>\r\n";
        private static MovieSearchItem[] movies;
        private static String expectedMoviesHtml = "<table class='table table-hover'>\n<thead>\n<tr>\r\n<th>Id</th>\r\n<th>Title</th>\r\n<th>ReleaseDate</th>\r\n</tr>\n<thead>\n<tbody>\r\n<tr><td><a href='/movies/11'> 11 </a></td><td>Star Wars</td><td>1977-05-25</td></tr>\r\n</tbody>\n</table>\r\n";
        private static HtmlEmit htmlectEmit = new HtmlEmit();
        static HtmlectEmitTest()
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
        public void ToHtmlEmitTest()
        {
            Assert.AreEqual(htmlectEmit.ToHtml(movie), expectedMovieHtml);
        }

        [TestMethod]
        public void ToHtmlEmitArrayTest()
        {
            Assert.AreEqual(htmlectEmit.ToHtml(movies), expectedMoviesHtml);
        }
    }
}
