namespace Runtime.Core.Simulation
{
  using Architecture.Core.Simulation;

  /// <summary>
  /// Factory providing pre-populated deterministic state mock instances for tests and local simulation integration.
  /// </summary>
  public static class MockSimulationContextFactory
  {
    public static SimulationContext CreateInitialTwoPlayerDuel()
    {
      var context = new SimulationContext();

      // Spawn Player 1 Champion
      var unitP1 = new EntityState(entityId : MockSimulationConstants.PlayerOneEntityId , ownerId : SimulationConstants.DefaultInitialPlayerId , nodeSector : MockSimulationConstants.PlayerOneSector , nodeIndex : MockSimulationConstants.PlayerOneNodeIndex , health : MockSimulationConstants.DefaultUnitHealth , energy : MockSimulationConstants.DefaultUnitEnergy);
      context.SetEntityState(unitP1.EntityId , unitP1);
      context.SetNodeOccupant(unitP1.NodeSector , unitP1.NodeIndex , unitP1.EntityId);

      // Spawn Player 2 Champion
      var unitP2 = new EntityState(entityId : MockSimulationConstants.PlayerTwoEntityId , ownerId : SimulationConstants.SecondaryPlayerId , nodeSector : MockSimulationConstants.PlayerTwoSector , nodeIndex : MockSimulationConstants.PlayerTwoNodeIndex , health : MockSimulationConstants.DefaultUnitHealth , energy : MockSimulationConstants.DefaultUnitEnergy);
      context.SetEntityState(unitP2.EntityId , unitP2);
      context.SetNodeOccupant(unitP2.NodeSector , unitP2.NodeIndex , unitP2.EntityId);

      context.AdvanceTurn(nextPlayerId : SimulationConstants.DefaultInitialPlayerId);
      return context;
    }

    public static SimulationContext CreateEmptyContext(uint startingTick = SimulationConstants.InitialTick , byte activePlayerId = SimulationConstants.DefaultInitialPlayerId)
    {
      var context = new SimulationContext();
      context.SetTick(startingTick);
      context.AdvanceTurn(activePlayerId);
      return context;
    }
  }
}