namespace Architecture.Contracts
{
  /// <summary>
  /// View contract for displaying simulation telemetry state such as active tick and executed commands.
  /// </summary>
  public interface ISimulationTelemetryView
  {
    uint DisplayedTick{get;}
    ICommand LastReceivedCommand{get;}

    void UpdateTickDisplay(uint tick);
    void UpdateCommandDisplay(ICommand command);
  }
}