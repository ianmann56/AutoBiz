namespace Todos.Business
{
  public class Todo
  {
    public string Name { get; }
    public bool IsComplete { get; }

    public Todo(string name, bool isComplete)
    {
      Name = name;
      IsComplete = isComplete;
    }
  }
}