namespace Runtime.Core.Simulation
{
  using System;
  using System.Collections.Generic;

  public sealed class SimulationContext : ISimulationContext
  {
    public uint CurrentTick{get; private set;}
    public byte ActivePlayerId{get; private set;}
    public int TurnNumber{get; private set;}

    private readonly Dictionary<ushort , EntityState> _entities;
    private readonly ushort[,] _nodeGrid;

    public SimulationContext(int initialCapacity = SimulationConstants.DefaultEntityCapacity)
    {
      _entities = new Dictionary<ushort , EntityState>(initialCapacity);
      _nodeGrid = new ushort[SimulationConstants.MaxSectors , SimulationConstants.MaxNodesPerSector];
    }

    public bool TryGetEntityState(ushort entityId , out EntityState state) { return _entities.TryGetValue(entityId , out state); }

    public void SetEntityState(ushort entityId , in EntityState state) { _entities[entityId] = state; }

    public bool TryGetNodeOccupant(byte nodeSector , byte nodeIndex , out ushort entityId)
    {
      if(nodeSector >= SimulationConstants.MaxSectors || nodeIndex >= SimulationConstants.MaxNodesPerSector)
      {
        entityId = SimulationConstants.EmptyNodeOccupantId;
        return false;
      }

      entityId = _nodeGrid[nodeSector , nodeIndex];
      return entityId != SimulationConstants.EmptyNodeOccupantId;
    }

    public void SetNodeOccupant(byte nodeSector , byte nodeIndex , ushort entityId)
    {
      if(nodeSector < SimulationConstants.MaxSectors && nodeIndex < SimulationConstants.MaxNodesPerSector) { _nodeGrid[nodeSector , nodeIndex] = entityId; }
    }

    public void ClearNodeOccupant(byte nodeSector , byte nodeIndex)
    {
      if(nodeSector < SimulationConstants.MaxSectors && nodeIndex < SimulationConstants.MaxNodesPerSector) { _nodeGrid[nodeSector , nodeIndex] = SimulationConstants.EmptyNodeOccupantId; }
    }

    public void AdvanceTick() { CurrentTick++; }

    public void SetTick(uint tick) { CurrentTick = tick; }

    public void AdvanceTurn(byte nextPlayerId)
    {
      TurnNumber++;
      ActivePlayerId = nextPlayerId;
    }

    public void ResetTo(in ISimulationContext sourceContext)
    {
      CurrentTick = sourceContext.CurrentTick;
      ActivePlayerId = sourceContext.ActivePlayerId;
      TurnNumber = sourceContext.TurnNumber;
      _entities.Clear();
      Array.Clear(_nodeGrid , 0 , _nodeGrid.Length);
    }
  }
}