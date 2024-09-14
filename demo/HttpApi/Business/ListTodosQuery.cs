using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Todos.Business
{
  public class ListTodosQueryArguments
  {
    public string? Filter { get; init; } = string.Empty;
  }

  public class ListTodosQueryDeps
  {
    public ITodoRepository TodoRepository { get; init; } = default!;
  }
  
  public static class ListTodosQuery
  {
    public static async Task<IEnumerable<Todo>> Query(ListTodosQueryArguments args, ListTodosQueryDeps deps)
    {
      IEnumerable<Todo> all = await deps.TodoRepository.ListTodos();
      return string.IsNullOrWhiteSpace(args.Filter)
        ? all
        : all.Where(t => t.Name.Contains(args.Filter, StringComparison.OrdinalIgnoreCase)).ToList();
    }
  }
}