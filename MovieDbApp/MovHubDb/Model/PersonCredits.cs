using Newtonsoft.Json;

namespace MovHubDb.Model
{
    public class PersonCredits
    {
        private PersonCreditsItem[] cast, crew;
        
        [JsonProperty("cast")]
        public PersonCreditsItem[] Cast
        {
            get => cast;
            set => cast = value;
        }
        
        [JsonProperty("crew")]
        public PersonCreditsItem[] Crew
        {
            get => crew;
            set => crew = value;
        }
        
        
        public PersonCreditsItem[] All
        {
            get => getAllItems();
        }
        
        private PersonCreditsItem[] getAllItems()
        {
            if (cast == null)
                return crew;
            if (crew == null)
                return cast;
            
            PersonCreditsItem[] all = new PersonCreditsItem[cast.Length + crew.Length];
            int i = 0;
            foreach (var VARIABLE in cast)
            {
                all[i] = VARIABLE;
                ++i;
            }
            foreach (var VARIABLE in crew)
            {
                all[i] = VARIABLE;
                ++i;
            }
            return all;
        }
    }
}