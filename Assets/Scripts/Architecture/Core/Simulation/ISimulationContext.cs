namespace Runtime.Core.Simulation
{
  using System;

  /// <summary>
  /// Contract representing the deterministic game state context passed into commands during execution.
  /// </summary>
  public interface ISimulationContext
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

  /// <summary>
  /// Value-type entity snapshot matching zero-allocation network delta requirements.
  /// </summary>
  public readonly struct EntityState : IEquatable<EntityState>
  {
    public readonly ushort EntityId;
    public readonly byte OwnerId;
    public readonly byte NodeSector;
    public readonly byte NodeIndex;
    public readonly ushort Health;
    public readonly ushort Energy;

    public EntityState(ushort entityId , byte ownerId , byte nodeSector , byte nodeIndex , ushort health , ushort energy)
    {
      EntityId = entityId;
      OwnerId = ownerId;
      NodeSector = nodeSector;
      NodeIndex = nodeIndex;
      Health = health;
      Energy = energy;
    }

    public bool Equals(EntityState other) { return EntityId == other.EntityId && OwnerId == other.OwnerId && NodeSector == other.NodeSector && NodeIndex == other.NodeIndex && Health == other.Health && Energy == other.Energy; }

    public override bool Equals(object obj) => obj is EntityState other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(EntityId , OwnerId , NodeSector , NodeIndex , Health , Energy);
  }
}