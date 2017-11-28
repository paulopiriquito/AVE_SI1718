using Newtonsoft.Json;

namespace MovHubDb.Model
{
    public class Credits
    {
        private CreditsItem[] cast, crew;
        
        [JsonProperty("cast")]
        public CreditsItem[] Cast
        {
            get => cast;
            set => cast = value;
        }
        
        [JsonProperty("crew")]
        public CreditsItem[] Crew
        {
            get => crew == null ? null : crew;
            set => crew = value;
        }

        public CreditsItem[] AllItems
        {
            get => getAllItems();
        }

        private CreditsItem[] getAllItems()
        {
            if (cast == null)
                return crew;
            if (crew == null)
                return cast;
            
            CreditsItem[] all = new CreditsItem[cast.Length + crew.Length];
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