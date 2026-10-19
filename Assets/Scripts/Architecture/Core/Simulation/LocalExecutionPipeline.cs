namespace Architecture.Core.Simulation
{
  using Contracts;
  using System;
  using System.Collections.Generic;
  using UnityEngine;

  [DefaultExecutionOrder(SimulationConstants.PipelineExecutionOrder)]
  public class LocalExecutionPipeline : MonoBehaviour , ILocalExecutionPipeline
  {
    private readonly int _maxCommandsPerTick = SimulationConstants.DefaultMaxCommandsPerTick;

    private CommandProcessor _processor;
    private readonly List<ICommand> _executedHistory = new();

    [SerializeField] private bool autoStepOnUpdate;

    public bool IsProcessing{get; private set;}
    public uint CurrentTick{get; private set;}

    public event Action<ICommand> OnCommandExecuting;
    public event Action<ICommand , bool> OnCommandExecuted;
    public event Action<uint> OnTickCompleted;

    /// <summary>
    /// Initializes the pipeline with the required game state context.
    /// Can be called on Awake or injected during setup.
    /// </summary>
    public void Initialize(IGameStateContext context) { _processor = new CommandProcessor(context); }

    public void EnqueueCommand(ICommand command)
    {
      if(command == null)
      {
        Debug.LogWarning("[LocalExecutionPipeline] Attempted to enqueue a null command.");
        return;
      }

      if(_processor == null)
      {
        Debug.LogError("[LocalExecutionPipeline] Pipeline has not been initialized with an IGameStateContext!");
        return;
      }

      _processor.Enqueue(command);
    }

    public void StepSimulation()
    {
      if(IsProcessing)
      {
        Debug.LogWarning("[LocalExecutionPipeline] Re-entrant simulation step detected. Aborting step.");
        return;
      }

      if(_processor == null)
      {
        Debug.LogError("[LocalExecutionPipeline] Cannot step simulation: CommandProcessor is not initialized.");
        return;
      }

      IsProcessing = true;
      int processedCount = 0;

      try
      {
        while(_processor.QueuedCount > 0 && processedCount < _maxCommandsPerTick)
        {
          // CommandProcessor.ProcessNext executes the command directly internally
          bool executed = _processor.ProcessNext();
          if(executed) { processedCount++; }
        }

        CurrentTick++;
        OnTickCompleted?.Invoke(CurrentTick);
      }
      catch(Exception ex) { Debug.LogError($"[LocalExecutionPipeline] Error stepping simulation tick {CurrentTick}: {ex.Message}\n{ex.StackTrace}"); }
      finally { IsProcessing = false; }
    }

    public void ClearBuffer() { _processor?.Clear(); }

    private void Update()
    {
      if(autoStepOnUpdate && _processor != null && _processor.QueuedCount > 0) { StepSimulation(); }
    }

    public IReadOnlyList<ICommand> GetExecutionHistory() => _executedHistory.AsReadOnly();

    // Invocation helpers for external dispatchers or unit test runners
    protected void RaiseCommandExecuting(ICommand command) => OnCommandExecuting?.Invoke(command);
    protected void RaiseCommandExecuted(ICommand command , bool success) => OnCommandExecuted?.Invoke(command , success);
  }
}