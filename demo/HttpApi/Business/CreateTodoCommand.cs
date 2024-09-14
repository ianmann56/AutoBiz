using System.Threading.Tasks;

namespace Todos.Business
{
  public class CreateTodoCommandArguments
  {
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
  }

  public class CreateTodoCommandDeps
  {
    public ITodoRepository TodoRepository { get; init; } = default!;
  }
  
  public static class CreateTodoCommand
  {
    public static Task Execute(CreateTodoCommandArguments args, CreateTodoCommandDeps deps)
    {
      var todo = new CreateTodoDto(args.Title, args.Description);
      return deps.TodoRepository.CreateTodo(todo);
    }
  }
}