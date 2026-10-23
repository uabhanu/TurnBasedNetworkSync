namespace Architecture.Core.Simulation
{
  using System;

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