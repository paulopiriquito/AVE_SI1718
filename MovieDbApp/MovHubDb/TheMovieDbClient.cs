using MovHubDb.Model;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Text;

namespace MovHubDb
{
    public class TheMovieDbClient
    {
        private const String API_KEY = "d109c79ba0d619a3a74f33532eb3aa06";
        private readonly WebClient client = new WebClient { Encoding = Encoding.UTF8 };
        
        /// <summary>
        /// e.g.: https://api.themoviedb.org/3/search/movie?api_key=d109c79ba0d619a3a74f33532eb3aa06&query=war%20games
        /// </summary>
        public MovieSearchItem[] Search(string title, int page)
        {
            String request = "https://api.themoviedb.org/3/search/movie?query="+title.Replace(" ","+")+"&include_adult=false&page="+page+"&api_key="+API_KEY;
            String response = client.DownloadString(request);
            MovieSearch results = JsonConvert.DeserializeObject<MovieSearch>(response);
            return results.Results;
        }

        /// <summary>
        /// e.g.: https://api.themoviedb.org/3/movie/508?api_key=d109c79ba0d619a3a74f33532eb3aa06
        /// </summary>
        public Movie MovieDetails(int id)
        {
            String request = "https://api.themoviedb.org/3/movie/"+id+"?api_key="+API_KEY;
            String response = client.DownloadString(request);
            Movie movie = JsonConvert.DeserializeObject<Movie>(response);
            return movie;
        }

        /// <summary>
        /// e.g.: https://api.themoviedb.org/3/movie/508/credits?api_key=d109c79ba0d619a3a74f33532eb3aa06
        /// </summary>
        public CreditsItem[] MovieCredits(int id) {
            
            String request = "https://api.themoviedb.org/3/movie/"+id+"/credits?api_key="+API_KEY;
            String response = client.DownloadString(request);
            Credits credits = JsonConvert.DeserializeObject<Credits>(response);
            return credits.AllItems;
        }

        /// <summary>
        /// e.g.: https://api.themoviedb.org/3/person/3489?api_key=d109c79ba0d619a3a74f33532eb3aa06
        /// </summary>
        public Person PersonDetais(int actorId)
        {
            String request = "https://api.themoviedb.org/3/person/"+actorId+"?api_key="+API_KEY;
            String response = client.DownloadString(request);
            Person person = JsonConvert.DeserializeObject<Person>(response);
            return person;
        }

        /// <summary>
        /// e.g.: https://api.themoviedb.org/3/person/3489/movie_credits?api_key=d109c79ba0d619a3a74f33532eb3aa06
        /// </summary>
        public PersonCreditsItem[] PersonMovies(int actorId) {
            String request = "https://api.themoviedb.org/3/person/"+actorId+"/movie_credits?api_key="+API_KEY;
            String response = client.DownloadString(request);
            PersonCredits results = JsonConvert.DeserializeObject<PersonCredits>(response);
            return results.Cast;
        }
    }
}
