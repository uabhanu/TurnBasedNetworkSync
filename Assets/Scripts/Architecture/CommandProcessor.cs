namespace Architecture
{
  using Contracts;
  using System;
  using System.Collections.Generic;

  public class CommandProcessor
  {
    private readonly Queue<ICommand> _commandQueue;
    private readonly IGameStateContext _context;

    public CommandProcessor(IGameStateContext context)
    {
      _commandQueue = new Queue<ICommand>();
      _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public int QueuedCount => _commandQueue.Count;

    public void Enqueue(ICommand command)
    {
      if(command == null) { throw new ArgumentNullException(nameof(command)); }

      _commandQueue.Enqueue(command);
    }

    public bool ProcessNext()
    {
      if(_commandQueue.Count == 0) { return false; }

      ICommand command = _commandQueue.Dequeue();
      command.Execute(_context);
      return true;
    }

    public int ProcessAll()
    {
      int processedCount = 0;

      while(_commandQueue.Count > 0)
      {
        ICommand command = _commandQueue.Dequeue();
        command.Execute(_context);
        processedCount++;
      }

      return processedCount;
    }

    public void Clear() { _commandQueue.Clear(); }
  }
}