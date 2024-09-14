using System;
using System.Threading.Tasks;

namespace Todos.Business
{
  public class DeleteTodoContext
  {
    public Guid Id { get; init; }
  }

  public class DeleteTodoDeps
  {
    public ITodoRepository TodoRepository { get; init; } = null!;
  }

  public static class DeleteTodoCommand
  {
    public static Task Execute(DeleteTodoContext context, DeleteTodoDeps deps)
    {
      return deps.TodoRepository.DeleteTodo(context.Id);
    }
  }
}