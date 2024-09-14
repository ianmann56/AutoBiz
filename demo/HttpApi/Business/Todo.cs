using System;

namespace Todos.Business
{
  public class Todo
  {
    public Guid Id { get; }
    public string Name { get; }
    public string Description { get; }
    public bool IsComplete { get; }

    public Todo(Guid id, string name, string description, bool isComplete)
    {
      Id = id;
      Name = name;
      Description = description;
      IsComplete = isComplete;
    }
  }
}