﻿using System.ComponentModel.DataAnnotations;

namespace TodoList.Dtos
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public List<Todo> Todos { get; set; } = new List<Todo>();
    }
}
