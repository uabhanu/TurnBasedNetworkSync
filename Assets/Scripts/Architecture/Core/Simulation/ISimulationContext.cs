namespace Architecture.Core.Simulation
{
  using Contracts;

  /// <summary>
  /// Contract representing the deterministic game state context passed into commands during execution.
  /// </summary>
  public interface ISimulationContext : IGameStateContext
  {
    uint CurrentTick{get;}
    byte ActivePlayerId{get;}
    int TurnNumber{get;}

    bool TryGetEntityState(ushort entityId , out EntityState state);
    void SetEntityState(ushort entityId , in EntityState state);

    bool TryGetNodeOccupant(byte nodeSector , byte nodeIndex , out ushort entityId);
    void SetNodeOccupant(byte nodeSector , byte nodeIndex , ushort entityId);
    void ClearNodeOccupant(byte nodeSector , byte nodeIndex);

    void AdvanceTick();
    void AdvanceTurn(byte nextPlayerId);
    void ResetTo(in ISimulationContext sourceContext);
    void SetTick(uint tick);
  }
}