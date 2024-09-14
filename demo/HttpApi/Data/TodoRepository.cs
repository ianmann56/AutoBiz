using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Todos.Business;

namespace Todos.Data
{
  public class TodoRepository : ITodoRepository
  {
    private readonly List<Todo> todos = new();
    
    public Task CreateTodo(CreateTodoDto data)
    {
      Todo todo = new Todo(Guid.NewGuid(), data.Name, data.Description, false);
      this.todos.Add(todo);
      return Task.CompletedTask;
    }

    public Task<IEnumerable<Todo>> ListTodos()
    {
      return Task.FromResult<IEnumerable<Todo>>(this.todos);
    }

    public Task DeleteTodo(Guid id)
    {
      Todo? todo = this.todos.SingleOrDefault(t => t.Id == id);
      if (todo != null)
      {
        this.todos.Remove(todo);
      }
      return Task.CompletedTask;
    }
  }
}