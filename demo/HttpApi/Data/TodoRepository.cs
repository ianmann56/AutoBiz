using System.Collections.Generic;
using System.Threading.Tasks;
using Todos.Business;

namespace Todos.Data
{
  public class TodoRepository : ITodoRepository
  {
    private readonly List<Todo> todos = new();
    
    public Task CreateTodo(CreateTodoDto data)
    {
      Todo todo = new Todo(data.Name, false);
      this.todos.Add(todo);
      return Task.CompletedTask;
    }
  }
}