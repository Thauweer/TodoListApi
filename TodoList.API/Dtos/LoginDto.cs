using Microsoft.AspNetCore.Mvc;

namespace ToDoListApi.Dtos
{
    public class LoginDto
    {
        public string NickName { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
    }
}
