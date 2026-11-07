namespace Runtime.UI
{
  using Architecture.Contracts;
  using Architecture.Events;
  using UnityEngine;

  [AddComponentMenu("TurnBasedSync/UI/Simulation Telemetry View")]
  public class SimulationTelemetryView : MonoBehaviour , ISimulationTelemetryView
  {
    [SerializeField] private ScriptableEventICommand commandSubmittedEvent;
    [SerializeField] private ScriptableEventUint tickDispatchedEvent;

    public uint DisplayedTick{get; private set;}
    public ICommand LastReceivedCommand{get; private set;}

    public ScriptableEventUint TickDispatchedEvent{get => tickDispatchedEvent; set => tickDispatchedEvent = value;}

    public ScriptableEventICommand CommandSubmittedEvent{get => commandSubmittedEvent; set => commandSubmittedEvent = value;}

    private SimulationTelemetryPresenter _presenter;

    private void Awake() { _presenter = new SimulationTelemetryPresenter(this , tickDispatchedEvent , commandSubmittedEvent); }

    private void OnEnable() { _presenter?.Enable(); }

    private void OnDisable() { _presenter?.Disable(); }

    public void UpdateTickDisplay(uint tick) => DisplayedTick = tick;

    public void UpdateCommandDisplay(ICommand command) => LastReceivedCommand = command;
  }
}