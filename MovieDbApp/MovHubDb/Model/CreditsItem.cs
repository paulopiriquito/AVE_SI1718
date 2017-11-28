using Newtonsoft.Json;
using HtmlReflect;

namespace MovHubDb.Model
{
    public class CreditsItem
    {
        private int id;
        private string character, name, department;

        [HtmlAs("<a href='/person/{value}/movies'> {value} </a>")]
        [JsonProperty("id")]
        public int Id
        {
            get => id;
            set => id = value;
        }
        
        [JsonProperty("name")]
        public string Name
        {
            get => name;
            set => name = value;
        }
        
        //[HtmlIgnore]
        [JsonProperty("character")]
        public string Character
        {
            get => character;
            set => character = value;
        }
        
        //[HtmlIgnore]
        [JsonProperty("department")]
        public string Department
        {
            get => department;
            set => department = value;
        }
        
        
        [HtmlIgnore]
        public bool IsCast
        {
            get => character != null;
        }
        
        [HtmlIgnore]
        public bool IsCrew
        {
            get => department != null;
        }

    }
}