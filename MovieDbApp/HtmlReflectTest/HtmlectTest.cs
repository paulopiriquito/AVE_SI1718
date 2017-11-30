using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovHubDb;
using HtmlReflect;
using MovHubDb.Model;

namespace HtmlReflectTest
{
    [TestClass]
    public class HtmlectTest
    {
        static Movie movie = new TheMovieDbClient().MovieDetails(11);
        static String expectedMovie = "<ul class=\'list-group\'>\n<li class=\'list-group-item\'><strong>OriginalTitle</strong>: Star Wars</li>\n<li class=\'list-group-item\'><strong>TagLine</strong>: A long time ago in a galaxy far, far away...</li>\n<li class=\'list-group-item\'><a href=\'/movies/11/credits\'>Cast and crew </a></li><li class=\'list-group-item\'><strong>Budget</strong>: 11000000</li>\n<li class=\'list-group-item\'><strong>VoteAverage</strong>: 8,1</li>\n<li class=\'list-group-item\'><strong>ReleaseDate</strong>: 1977-05-25</li>\n<div class=\'card-body bg-light\'><div><strong>Overview</strong>:</div>Princess Leia is captured and held hostage by the evil Imperial forces in their effort to take over the galactic Empire. Venturesome Luke Skywalker and dashing captain Han Solo team together with the loveable robot duo R2-D2 and C-3PO to rescue the beautiful princess and restore peace and justice in the Empire.</div><div style=\"position:absolute; top:0; right:0;\"><img width=\"50%\" src=\"http://image.tmdb.org/t/p/w185//btTdmkgIvOi0FFip1sPuZI2oQG6.jpg\"></div></ul>";
        static MovieSearchItem[] movies = new TheMovieDbClient().Search("Star Wars", 1);
        static String expectedMovies = "<table class=\'table table-hover\'>\n<thead>\n<tr>\n<th>Id</th><th>Title</th><th>ReleaseDate</th></tr>\n<thead>\n<tbody>\n<tr><td><a href=\'/movies/11\'> 11 </a></td><td>Star Wars</td><td>1977-05-25</td></tr>\n<tr><td><a href=\'/movies/140607\'> 140607 </a></td><td>Star Wars: The Force Awakens</td><td>2015-12-15</td></tr>\n<tr><td><a href=\'/movies/181808\'> 181808 </a></td><td>Star Wars: The Last Jedi</td><td>2017-12-13</td></tr>\n<tr><td><a href=\'/movies/12180\'> 12180 </a></td><td>Star Wars: The Clone Wars</td><td>2008-08-05</td></tr>\n<tr><td><a href=\'/movies/330459\'> 330459 </a></td><td>Rogue One: A Star Wars Story</td><td>2016-12-14</td></tr>\n<tr><td><a href=\'/movies/42979\'> 42979 </a></td><td>Robot Chicken: Star Wars</td><td>2007-07-17</td></tr>\n<tr><td><a href=\'/movies/1893\'> 1893 </a></td><td>Star Wars: Episode I - The Phantom Menace</td><td>1999-05-19</td></tr>\n<tr><td><a href=\'/movies/181812\'> 181812 </a></td><td>Star Wars: Episode IX</td><td>2019-12-18</td></tr>\n<tr><td><a href=\'/movies/70608\'> 70608 </a></td><td>Lego Star Wars: The Padawan Menace</td><td>2011-07-22</td></tr>\n<tr><td><a href=\'/movies/1895\'> 1895 </a></td><td>Star Wars: Episode III - Revenge of the Sith</td><td>2005-05-17</td></tr>\n<tr><td><a href=\'/movies/1894\'> 1894 </a></td><td>Star Wars: Episode II - Attack of the Clones</td><td>2002-05-15</td></tr>\n<tr><td><a href=\'/movies/74849\'> 74849 </a></td><td>The Star Wars Holiday Special</td><td>1978-12-01</td></tr>\n<tr><td><a href=\'/movies/479310\'> 479310 </a></td><td>Star Wars Rebels: Heroes of Mandalore</td><td>2017-10-16</td></tr>\n<tr><td><a href=\'/movies/42982\'> 42982 </a></td><td>Robot Chicken: Star Wars Episode II</td><td>2008-11-16</td></tr>\n<tr><td><a href=\'/movies/76180\'> 76180 </a></td><td>Empire of Dreams: The Story of the Star Wars Trilogy</td><td>2004-09-12</td></tr>\n<tr><td><a href=\'/movies/51888\'> 51888 </a></td><td>Robot Chicken: Star Wars Episode III</td><td>2010-12-19</td></tr>\n<tr><td><a href=\'/movies/332479\'> 332479 </a></td><td>Star Wars: TIE Fighter</td><td>2015-03-24</td></tr>\n<tr><td><a href=\'/movies/1891\'> 1891 </a></td><td>The Empire Strikes Back</td><td>1980-05-17</td></tr>\n<tr><td><a href=\'/movies/333365\'> 333365 </a></td><td>Star Wars: Clone Wars (Volume 2)</td><td>2005-11-30</td></tr>\n<tr><td><a href=\'/movies/333355\'> 333355 </a></td><td>Star Wars: Clone Wars (Volume 1)</td><td>2005-03-21</td></tr>\n</tbody>\n</table>";
        static Htmlect htmlect = new Htmlect();

        [TestMethod]
        public void ToHtmlTest()
        {
            Assert.AreEqual(htmlect.ToHtml(movie), expectedMovie);
        }

        [TestMethod]
        public void ToHtmlArrayTest()
        {
            Assert.AreEqual(htmlect.ToHtml(movies), expectedMovies);
        }
    }
}




























