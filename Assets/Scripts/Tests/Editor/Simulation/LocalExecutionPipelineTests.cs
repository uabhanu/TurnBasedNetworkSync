namespace Tests.Editor.Simulation
{
  using Architecture.Contracts;
  using Architecture.Core.Simulation;
  using NUnit.Framework;
  using System;
  using UnityEngine;

  [TestFixture]
  public class LocalExecutionPipelineTests
  {
    private const uint ExpectedFirstTick = SimulationConstants.InitialTick + SimulationConstants.SingleTickStep;

    private GameObject _pipelineObject;
    private LocalExecutionPipeline _pipeline;
    private MockGameStateContext _context;

    private class MockGameStateContext : IGameStateContext {}

    private class MockCommand : ICommand
    {
      public CommandTypeId CommandTypeId{get;} = default;
      public byte PlayerId{get;} = SimulationConstants.DefaultInitialPlayerId;
      public int SequenceTag{get;}
      public int TickId{get;} = (int)SimulationConstants.InitialTick;
      public long Timestamp{get;}

      public bool HasExecuted{get; private set;}

      public void Execute(IGameStateContext context) { HasExecuted = true; }

      public byte[] Serialize() { return Array.Empty<byte>(); }
    }

    [SetUp]
    public void SetUp()
    {
      _pipelineObject = new GameObject("Test_Pipeline");
      _pipeline = _pipelineObject.AddComponent<LocalExecutionPipeline>();
      _context = new MockGameStateContext();
      _pipeline.Initialize(_context);
    }

    [TearDown]
    public void TearDown() { UnityEngine.Object.DestroyImmediate(_pipelineObject); }

    [Test]
    public void StepSimulation_ProcessesEnqueuedCommand_AndIncrementsTick()
    {
      var command = new MockCommand();
      _pipeline.EnqueueCommand(command);

      _pipeline.StepSimulation();

      Assert.That(command.HasExecuted , Is.True);
      Assert.That(_pipeline.CurrentTick , Is.EqualTo(ExpectedFirstTick));
    }

    [Test]
    public void StepSimulation_WithEmptyQueue_IncrementsTickWithoutError()
    {
      _pipeline.StepSimulation();

      Assert.That(_pipeline.CurrentTick , Is.EqualTo(ExpectedFirstTick));
      Assert.That(_pipeline.IsProcessing , Is.False);
    }

    [Test]
    public void ClearBuffer_DiscardsPendingCommands()
    {
      var command = new MockCommand();
      _pipeline.EnqueueCommand(command);

      _pipeline.ClearBuffer();
      _pipeline.StepSimulation();

      Assert.That(command.HasExecuted , Is.False);
      Assert.That(_pipeline.CurrentTick , Is.EqualTo(ExpectedFirstTick));
    }
  }
}