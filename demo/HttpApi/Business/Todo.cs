namespace Todos.Business
{
  public class Todo
  {
    public string Name { get; }
    public string Description { get; }
    public bool IsComplete { get; }

    public Todo(string name, string description, bool isComplete)
    {
      Name = name;
      Description = description;
      IsComplete = isComplete;
    }
  }
}