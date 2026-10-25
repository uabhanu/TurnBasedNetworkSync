namespace Architecture.Contracts
{
  /// <summary>
  /// Contract for scene-level or UI listeners binding to ScriptableObject simulation pipeline channels.
  /// </summary>
  public interface ISimulationPipelineListener
  {
    bool IsListening{get;}

    void HandleTickDispatched(uint tick);
    void HandleCommandSubmitted(ICommand command);
  }
}