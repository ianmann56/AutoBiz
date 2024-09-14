using System.Collections.Generic;
using System.Threading.Tasks;

namespace Todos.Business
{
  public class ListTodosQueryArguments
  {
    
  }

  public class ListTodosQueryDeps
  {
    public ITodoRepository TodoRepository { get; init; } = default!;
  }
  
  public static class ListTodosQuery
  {
    public static Task<IEnumerable<Todo>> Query(ListTodosQueryArguments args, ListTodosQueryDeps deps)
    {
      return deps.TodoRepository.ListTodos();
    }
  }
}