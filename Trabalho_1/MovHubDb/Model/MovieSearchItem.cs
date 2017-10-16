using HtmlReflect;
using Newtonsoft.Json;

namespace MovHubDb.Model
{
    public class MovieSearchItem
    {
        private int id;
        private float voteAverage;
        private string releaseDate, originalTitle;

        [HtmlAs("<a href='/movies/{value}'> {value} </a>")]
        [JsonProperty("id")]
        public int Id
        {
            get => id;
            set => id = value;
        }
        
        [JsonProperty("original_title")]
        public string Title
        {
            get => originalTitle;
            set => originalTitle = value;
        }

        [JsonProperty("release_date")]
        public string ReleaseDate
        {
            get => releaseDate;
            set => releaseDate = value;
        }
        
        [HtmlIgnore]
        [JsonProperty("vote_average")]
        public float VoteAverage
        {
            get => voteAverage;
            set => voteAverage = value;
        }
    }
}