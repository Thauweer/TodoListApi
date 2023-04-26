using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace TodoList.Dtos
{
    public class Todo
    {
        [Key]
        public int id { get; set; }
        public string title { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public string color { get; set; } = string.Empty;
        public string planned_time { get; set; } = string.Empty;
        public string category { get; set; } = string.Empty;
        public List<string> importance { get; set; } = new List<string>();

        [JsonIgnore]
        public User user { get; set; }

       
    }
}
