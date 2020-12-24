using System;
using Newtonsoft.Json;

namespace OnlineExamination.Models
{
    public class courseClass
    {
        int course_id;
        int User_id;
        string course_name;
        int fin;
        string User_nickname;
        [JsonProperty(PropertyName = "course_id")]
        public int Course_id { get => course_id; set => course_id = value; }
        [JsonProperty(PropertyName = "User_id")]
        public int User_id1 { get => User_id; set => User_id = value; }
        [JsonProperty(PropertyName = "course_name")]
        public string Course_name { get => course_name; set => course_name = value; }
        [JsonProperty(PropertyName = "fin")]
        public int Fin { get => fin; set => fin = value; }
        [JsonProperty(PropertyName = "User_nickname")]
        public string User_nickname1 { get => User_nickname; set => User_nickname = value; }
    }
}
