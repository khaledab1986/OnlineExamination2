using System;
using Newtonsoft.Json;

namespace OnlineExamination.Models
{
    public class UserModel
    {
        int id;
        string user_name;
        string user_pass;
        int user_kind; 
        int fin;
        [JsonProperty(PropertyName = "id")]
        public int Id { get => id; set => id = value; }
        [JsonProperty(PropertyName = "User_name")]
        public string User_name { get => user_name; set => user_name = value; }
        [JsonProperty(PropertyName = "User_pass")]
        public string User_pass { get => user_pass; set => user_pass = value; }
        [JsonProperty(PropertyName = "User_kind")]
        public int User_kind { get => user_kind; set => user_kind = value; }
        [JsonProperty(PropertyName = "fin")]
        public int Fin { get => fin; set => fin = value; }
        
        
    }
}
