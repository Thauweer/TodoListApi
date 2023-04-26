using System.ComponentModel.DataAnnotations;

namespace TodoList.Dtos
{
    public class TodoForm : ITodo
    {
        [Required(ErrorMessage = "Title is required.")]
        public string title { get; set; } = string.Empty;
        [Required(ErrorMessage = "Description is required.")]
        public string description { get; set; } = string.Empty;
    }
}
