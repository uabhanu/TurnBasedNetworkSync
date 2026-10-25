namespace Architecture.Core.Simulation
{
  using Contracts;
  using UnityEngine;
  using UnityEngine.Events;

  /// <summary>
  /// Decouples simulation event dispatches from scene/UI views via ScriptableObject channel subscriptions.
  /// </summary>
  [AddComponentMenu("TurnBasedSync/Architecture/Simulation Pipeline Listener")]
  public class SimulationPipelineListener : MonoBehaviour , ISimulationPipelineListener
  {
    [SerializeField] private SimulationPipelineChannel pipelineChannel;
    [SerializeField] private UnityEvent<uint> onTickDispatched;
    [SerializeField] private UnityEvent<ICommand> onCommandSubmitted;

    public bool IsListening => pipelineChannel && enabled;

    public SimulationPipelineChannel Channel
    {
      get => pipelineChannel;
      set
      {
        if(enabled && pipelineChannel) { Unsubscribe(); }

        pipelineChannel = value;

        if(enabled && pipelineChannel) { Subscribe(); }
      }
    }

    private void OnEnable() { Subscribe(); }

    private void OnDisable() { Unsubscribe(); }

    public void HandleTickDispatched(uint tick) { onTickDispatched?.Invoke(tick); }

    public void HandleCommandSubmitted(ICommand command) { onCommandSubmitted?.Invoke(command); }

    private void Subscribe()
    {
      if(!pipelineChannel) { return; }

      pipelineChannel.TickDispatched += HandleTickDispatched;
      pipelineChannel.CommandSubmitted += HandleCommandSubmitted;
    }

    private void Unsubscribe()
    {
      if(!pipelineChannel) { return; }

      pipelineChannel.TickDispatched -= HandleTickDispatched;
      pipelineChannel.CommandSubmitted -= HandleCommandSubmitted;
    }
  }
}