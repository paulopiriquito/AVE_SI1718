using HtmlReflect;
using Newtonsoft.Json;

namespace MovHubDb.Model
{
    public class PersonCreditsItem : MovieSearchItem
    {
        private string character, department;
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
        
        [JsonProperty("character")]
        public string Character
        {
            get => character;
            set => character = value;
        }
        
        [HtmlIgnore]
        [JsonProperty("department")]
        public string Department
        {
            get => character;
            set => character = value;
        }

        [JsonProperty("release_date")]
        public string ReleaseDate
        {
            get => releaseDate;
            set => releaseDate = value;
        }
        
        [JsonProperty("vote_average")]
        public float VoteAverage
        {
            get => voteAverage;
            set => voteAverage = value;
        } 
        
    }
}