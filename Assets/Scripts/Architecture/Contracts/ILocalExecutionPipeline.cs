namespace Architecture.Contracts
{
  using System;

  public interface ILocalExecutionPipeline
  {
    bool IsProcessing{get;}
    uint CurrentTick{get;}

    event Action<ICommand> OnCommandExecuting;
    event Action<ICommand , bool> OnCommandExecuted;
    event Action<uint> OnTickCompleted;

    void EnqueueCommand(ICommand command);
    void StepSimulation();
    void ClearBuffer();
  }
}