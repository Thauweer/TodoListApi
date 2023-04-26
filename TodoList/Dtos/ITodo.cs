namespace TodoList.Dtos
{
    public abstract class ITodo
    {
        public string? title { get; set; } = string.Empty;
        public string? description { get; set; } = string.Empty;
        public string? color { get; set; } = string.Empty;
        public string planned_time { get; set; } = string.Empty;
        public string category { get; set; } = string.Empty;
        public List<string> importance { get; set; } = new List<string>();
    }
}
