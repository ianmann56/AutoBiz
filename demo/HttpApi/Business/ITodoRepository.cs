using System.Threading.Tasks;

namespace Todos.Business
{
  public interface ITodoRepository
  {
    Task CreateTodo(CreateTodoDto data);
  }

  public record CreateTodoDto(string Name);
}