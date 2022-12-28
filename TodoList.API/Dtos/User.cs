using System.ComponentModel.DataAnnotations;

namespace ToDoListApi.Dtos
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public List<Todo> Todos { get; set; }
    }
}
