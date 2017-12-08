using Newtonsoft.Json;

namespace MovHubDb.Model
{
    public class MovieSearch
    {
        private MovieSearchItem[] results;
        
        [JsonProperty("results")]
        public MovieSearchItem[] Results
        {
            get => results;
            set => results = value;
        }
    }
}