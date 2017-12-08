using HtmlReflect;
using Newtonsoft.Json;

namespace MovHubDb.Model
{
    public class Movie
    {
        private string overview, releaseDate, originalTitle, tagLine, posterPath;
        private int id, budget, revenue;
        private float popularity, voteAverage;
        
        [JsonProperty("original_title")]
        public string OriginalTitle
        {
            get => originalTitle;
            set => originalTitle = value;
        }
                
        [JsonProperty("tagline")]
        public string TagLine
        {
            get => tagLine;
            set => tagLine = value;
        }
        
        [HtmlAs("<li class='list-group-item'><a href='/movies/{value}/credits'>Cast and crew </a></li>")]
        [JsonProperty("id")]
        public int Id
        {
            get => id;
            set => id = value;
        }
            
        [JsonProperty("budget")]
        public int Budget
        {
            get => budget;
            set => budget = value;
        }
                
        [JsonProperty("vote_average")]
        public float VoteAverage
        {
            get => voteAverage;
            set => voteAverage = value;
        }
        
        [JsonProperty("release_date")]
        public string ReleaseDate
        {
            get => releaseDate;
            set => releaseDate = value;
        }
        
        [HtmlAs("<div class='card-body bg-light'><div><strong>{name}</strong>:</div>{value}</div>")]
        [JsonProperty("overview")]
        public string Overview
        {
            get => overview;
            set => overview = value;
        }
        
        [HtmlIgnore]
        [JsonProperty("revenue")]
        public int Revenue
        {
            get => revenue;
            set => revenue = value;
        }
        
        [HtmlIgnore]
        [JsonProperty("popularity")]
        public float Popularity
        {
            get => popularity;
            set => popularity = value;
        }
        
        [HtmlAs("<div style=\"position:absolute; top:0; right:0;\"><img width=\"50%\" src=\"{value}\"></div>")]
        [JsonProperty("poster_path")]
        public string PosterPath
        {
            get => posterPath;
            set => posterPath = "http://image.tmdb.org/t/p/w185/" + value;
        }
        
    }
}