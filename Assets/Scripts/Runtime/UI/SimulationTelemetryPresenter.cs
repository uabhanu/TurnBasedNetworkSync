namespace Runtime.UI
{
  using Architecture.Contracts;
  using Architecture.Events;
  using System;

  public sealed class SimulationTelemetryPresenter : IDisposable
  {
    private readonly ISimulationTelemetryView _view;
    private readonly ScriptableEventUint _tickEvent;
    private readonly ScriptableEventICommand _commandEvent;

    public SimulationTelemetryPresenter(ISimulationTelemetryView view , ScriptableEventUint tickEvent , ScriptableEventICommand commandEvent)
    {
      _view = view ?? throw new ArgumentNullException(nameof(view));
      _tickEvent = tickEvent;
      _commandEvent = commandEvent;
    }

    public void Enable()
    {
      if(_tickEvent != null) { _tickEvent.OnRaised += HandleTickDispatched; }

      if(_commandEvent != null) { _commandEvent.OnRaised += HandleCommandSubmitted; }
    }

    public void Disable()
    {
      if(_tickEvent != null) { _tickEvent.OnRaised -= HandleTickDispatched; }

      if(_commandEvent != null) { _commandEvent.OnRaised -= HandleCommandSubmitted; }
    }

    private void HandleTickDispatched(uint tick) { _view.UpdateTickDisplay(tick); }

    private void HandleCommandSubmitted(ICommand command) { _view.UpdateCommandDisplay(command); }

    public void Dispose() { Disable(); }
  }
}