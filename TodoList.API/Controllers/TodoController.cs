using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using ToDoListApi.Dtos;

namespace ToDoListApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class TodoController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationContext _context;

        public TodoController(IConfiguration configuration, ApplicationContext context)
        {
            _configuration = configuration;
            _context = context;
        }


        [HttpGet("getall")]
        public IActionResult GetAll()
        {
            var user = _context.Users.First(user => user.Name == HttpContext.Items["username"]);
            var todos = _context.Todos.Where(todo => todo.user == user).Select(x => new { x.Id, x.Title, x.Description, x.Color }).ToList();

            return Json(todos);
        }

        [HttpPost("create")]
        public IActionResult Create(TodoDto todo)
        {
            var user = _context.Users.First( user => user.Name == HttpContext.Items["username"]);
            Todo todoToAdd = new Todo() { Title = todo.Title, Description = todo.Description, Color = todo.Color, user = user };

            _context.Todos.Add(todoToAdd);

            user.Todos.Add(todoToAdd);

            _context.Users.Update(user);
            _context.SaveChanges();
            return Json(todo);
        }

        [HttpPut("update")]
        public IActionResult Update(int id, TodoDto todo)
        {
            var user = _context.Users.First(user => user.Name == HttpContext.Items["username"]);
            var todoList = _context.Todos.Where(todo => todo.Id == id);

            if (todoList.Count() == 0)
            {
                return BadRequest("Todo not found.");
            }

            var todoToUpdate = todoList.First();

            if (todoToUpdate.user != user)
            {
                return BadRequest("Wrong todo-id..");
            }

            todoToUpdate.Title = todo.Title ?? todoToUpdate.Title;
            todoToUpdate.Description = todo.Description ?? todoToUpdate.Description;
            todoToUpdate.Color = todo.Color ?? todoToUpdate.Color;

            _context.Todos.Update(todoToUpdate);
            _context.SaveChanges();

            return Ok("Todo is updated.");
        }

        [HttpDelete("delete")]
        public IActionResult Delete(int id)
        {
            var user = _context.Users.First(user => user.Name == HttpContext.Items["username"]);
            var todoList = _context.Todos.Where(todo => todo.Id == id);

            if (todoList.Count() == 0)
            {
                return BadRequest("Todo not found.");
            }

            if(todoList.First().user != user)
            {
                return BadRequest("Wrong todo-id..");
            }

            _context.Remove(todoList.First());
            _context.SaveChanges();

            return Ok("Todo is deleted.");
        }
    }
}
