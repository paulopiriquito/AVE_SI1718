using HtmlReflect;
using Newtonsoft.Json;

namespace MovHubDb.Model
{
    public class Person
    {
        private string biography,placeOfBirth, birthday, deathday, name, profilePath;
        private int id;
        private float popularity;

        [JsonProperty("name")]
        public string Name
        {
            get => name;
            set => name = value;
        }
        
        [JsonProperty("birthday")]
        public string Birthday
        {
            get => birthday;
            set => birthday = value;
        }
        
        [JsonProperty("deathday")]
        public string Deathday
        {
            get => deathday;
            set => deathday = value;
        }

        [JsonProperty("biography")]
        public string Biography
        {
            get => biography;
            set => biography = value;
        }
        
        [JsonProperty("popularity")]
        public float Popularity
        {
            get => popularity;
            set => popularity = value;
        }
        
        [JsonProperty("place_of_birth")]
        public string PlaceOfBirth
        {
            get => placeOfBirth;
            set => placeOfBirth = value;
        }

        [HtmlAs("<div style=\"position:absolute; top:0; right:0;\"><img width=\"50%\" src=\"{value}\"></div>")]
        [JsonProperty("profile_path")]
        public string ProfilePath
        {
            get => profilePath;
            set => profilePath = "http://image.tmdb.org/t/p/w185/" + value;
        }
        
        [HtmlIgnore]
        [JsonProperty("id")]
        public int Id
        {
            get => id;
            set => id = value;
        }

    }
}
