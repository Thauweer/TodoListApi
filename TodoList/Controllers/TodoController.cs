using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoList.Dtos;

namespace TodoList.Controllers
{
    [Authorize]
    [Route("api/todo")]
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
            var todos = _context.Todos.Where(todo => todo.user == user).Select(x => x).ToList();

            return Json(todos);
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] TodoForm todo)
        {
            var user = _context.Users.First( user => user.Name == HttpContext.Items["username"]);

            var TodoToAdd = new Todo()
            {
                title = todo.title,
                description = todo.description,
                color = todo.color,
                planned_time = todo.planned_time,
                category = todo.category,
                importance = todo.importance
            };

            _context.Todos.Add(TodoToAdd);
            user.Todos.Add(TodoToAdd);
            _context.Users.Update(user);
            _context.SaveChanges();

            var result = _context.Todos.OrderBy(todo => todo.id).Last(todo => todo.user == user);
            return Json(result); ;
        }

        [HttpPut("update")]
        public IActionResult Update(int id, [FromBody] TodoForm todo)
        {
            var user = _context.Users.First(user => user.Name == HttpContext.Items["username"]);
            var todoList = _context.Todos.Where(todo => todo.id == id);

            if (todoList.Count() == 0)
            {
                return BadRequest("Todo not found.");
            }

            var todoToUpdate = todoList.First();

            if (todoToUpdate.user != user)
            {
                return BadRequest("Wrong todo id.");
            }

            todoToUpdate.title = todo.title;
            todoToUpdate.description = todo.description;
            todoToUpdate.color = todo.color;
            todoToUpdate.planned_time = todo.planned_time;
            todoToUpdate.category = todo.category;
            todoToUpdate.importance = todo.importance;

            _context.Todos.Update(todoToUpdate);
            _context.SaveChanges();

            return Ok("Todo is updated.");
        }

        [HttpDelete("delete")]
        public IActionResult Delete(int id)
        {
            var user = _context.Users.First(user => user.Name == HttpContext.Items["username"]);
            var todoList = _context.Todos.Where(todo => todo.id == id);

            if (todoList.Count() == 0)
            {
                return BadRequest("Todo not found.");
            }

            if(todoList.First().user != user)
            {
                return BadRequest("Wrong todo-id.");
            }

            _context.Remove(todoList.First());
            _context.SaveChanges();

            return Ok("Todo is deleted.");
        }
    }
}
