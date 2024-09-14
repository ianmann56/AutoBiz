using System.Collections.Generic;
using System.Threading.Tasks;

namespace Todos.Business
{
  public interface ITodoRepository
  {
    Task CreateTodo(CreateTodoDto data);
    Task<IEnumerable<Todo>> ListTodos();
  }

  public record CreateTodoDto(string Name, string Description);
}