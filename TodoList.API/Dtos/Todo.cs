using System.ComponentModel.DataAnnotations;

namespace ToDoListApi.Dtos
{
    public class Todo
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty ;
        public string Color { get; set; } = string.Empty;

        public User user { get; set; }
    }
}
