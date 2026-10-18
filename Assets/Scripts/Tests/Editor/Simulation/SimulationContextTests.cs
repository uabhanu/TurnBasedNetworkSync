namespace Tests.Editor.Simulation
{
  using NUnit.Framework;
  using Runtime.Core.Simulation;

  [TestFixture]
  public sealed class SimulationContextTests
  {
    [Test]
    public void CreateEmptyContext_InitializesWithDefaultValues()
    {
      var context = MockSimulationContextFactory.CreateEmptyContext();

      Assert.AreEqual(SimulationConstants.InitialTick , context.CurrentTick);
      Assert.AreEqual(SimulationConstants.DefaultInitialPlayerId , context.ActivePlayerId);
      Assert.AreEqual(1 , context.TurnNumber);
    }

    [Test]
    public void CreateEmptyContext_WithExplicitStartingTick_SetsTickDirectly()
    {
      const uint targetTick = 250;
      var context = MockSimulationContextFactory.CreateEmptyContext(startingTick : targetTick);

      Assert.AreEqual(targetTick , context.CurrentTick);
    }

    [Test]
    public void CreateInitialTwoPlayerDuel_SetsUpUnitsAndGridDeterministically()
    {
      var context = MockSimulationContextFactory.CreateInitialTwoPlayerDuel();

      // Verify Player 1 Champion State
      bool p1StateExists = context.TryGetEntityState(MockSimulationConstants.PlayerOneEntityId , out var p1State);
      Assert.IsTrue(p1StateExists);
      Assert.AreEqual(SimulationConstants.DefaultInitialPlayerId , p1State.OwnerId);
      Assert.AreEqual(MockSimulationConstants.PlayerOneSector , p1State.NodeSector);
      Assert.AreEqual(MockSimulationConstants.PlayerOneNodeIndex , p1State.NodeIndex);
      Assert.AreEqual(MockSimulationConstants.DefaultUnitHealth , p1State.Health);
      Assert.AreEqual(MockSimulationConstants.DefaultUnitEnergy , p1State.Energy);

      // Verify Player 1 Grid Node
      bool p1OccupantExists = context.TryGetNodeOccupant(MockSimulationConstants.PlayerOneSector , MockSimulationConstants.PlayerOneNodeIndex , out ushort p1OccupantId);
      Assert.IsTrue(p1OccupantExists);
      Assert.AreEqual(MockSimulationConstants.PlayerOneEntityId , p1OccupantId);

      // Verify Player 2 Champion State
      bool p2StateExists = context.TryGetEntityState(MockSimulationConstants.PlayerTwoEntityId , out var p2State);
      Assert.IsTrue(p2StateExists);
      Assert.AreEqual(SimulationConstants.SecondaryPlayerId , p2State.OwnerId);
      Assert.AreEqual(MockSimulationConstants.PlayerTwoSector , p2State.NodeSector);
      Assert.AreEqual(MockSimulationConstants.PlayerTwoNodeIndex , p2State.NodeIndex);
      Assert.AreEqual(MockSimulationConstants.DefaultUnitHealth , p2State.Health);
      Assert.AreEqual(MockSimulationConstants.DefaultUnitEnergy , p2State.Energy);

      // Verify Player 2 Grid Node
      bool p2OccupantExists = context.TryGetNodeOccupant(MockSimulationConstants.PlayerTwoSector , MockSimulationConstants.PlayerTwoNodeIndex , out ushort p2OccupantId);
      Assert.IsTrue(p2OccupantExists);
      Assert.AreEqual(MockSimulationConstants.PlayerTwoEntityId , p2OccupantId);
    }

    [Test]
    public void AdvanceTick_IncrementsTickDeterministically()
    {
      var context = MockSimulationContextFactory.CreateEmptyContext();

      context.AdvanceTick();
      context.AdvanceTick();

      Assert.AreEqual(SimulationConstants.InitialTick + 2 , context.CurrentTick);
    }

    [Test]
    public void ClearNodeOccupant_SetsOccupantToEmptyConstant()
    {
      var context = MockSimulationContextFactory.CreateInitialTwoPlayerDuel();

      context.ClearNodeOccupant(MockSimulationConstants.PlayerOneSector , MockSimulationConstants.PlayerOneNodeIndex);

      bool hasOccupant = context.TryGetNodeOccupant(MockSimulationConstants.PlayerOneSector , MockSimulationConstants.PlayerOneNodeIndex , out ushort occupantId);

      Assert.IsFalse(hasOccupant);
      Assert.AreEqual(SimulationConstants.EmptyNodeOccupantId , occupantId);
    }

    [Test]
    public void ResetTo_CopiesScalarsAndClearsNodeGrid()
    {
      var source = MockSimulationContextFactory.CreateInitialTwoPlayerDuel();
      var target = MockSimulationContextFactory.CreateEmptyContext();

      target.ResetTo(source);

      Assert.AreEqual(source.CurrentTick , target.CurrentTick);
      Assert.AreEqual(source.ActivePlayerId , target.ActivePlayerId);
      Assert.AreEqual(source.TurnNumber , target.TurnNumber);

      // Ensure grid in target was wiped
      bool hasOccupant = target.TryGetNodeOccupant(MockSimulationConstants.PlayerOneSector , MockSimulationConstants.PlayerOneNodeIndex , out ushort occupantId);

      Assert.IsFalse(hasOccupant);
      Assert.AreEqual(SimulationConstants.EmptyNodeOccupantId , occupantId);
    }
  }
}