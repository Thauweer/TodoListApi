using Microsoft.AspNetCore.Mvc;

namespace TodoList.Dtos
{
    public class LoginDto
    {
        public string Login { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
    }
}
