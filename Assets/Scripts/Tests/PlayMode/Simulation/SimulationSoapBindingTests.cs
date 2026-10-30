namespace Tests.PlayMode.Simulation
{
  using Architecture.Contracts;
  using Architecture.Core.Simulation;
  using Architecture.Events;
  using NUnit.Framework;
  using Runtime.Core.Simulation;
  using Runtime.UI;
  using UnityEngine;

  [TestFixture]
  public class SimulationSoapBindingTests
  {
    private ScriptableEventICommand _commandEvent;
    private ScriptableEventUint _tickEvent;
    private GameObject _pipelineObject;
    private LocalExecutionPipeline _pipeline;
    private MockTelemetryView _mockView;
    private SimulationTelemetryPresenter _presenter;

    private class MockTelemetryView : ISimulationTelemetryView
    {
      public uint DisplayedTick{get; private set;}
      public ICommand LastReceivedCommand{get; private set;}

      public void UpdateTickDisplay(uint tick) => DisplayedTick = tick;
      public void UpdateCommandDisplay(ICommand command) => LastReceivedCommand = command;
    }

    private class MockTestCommand : ICommand
    {
      public CommandTypeId CommandTypeId{get;} = CommandTypeId.MockIncrement;
      public byte PlayerId{get;} = SimulationConstants.DefaultInitialPlayerId;
      public int SequenceTag{get;}
      public int TickId{get;} = (int)SimulationConstants.InitialTick;
      public long Timestamp{get;}

      public void Execute(IGameStateContext context) {}
      public byte[] Serialize() => System.Array.Empty<byte>();
    }

    [SetUp]
    public void SetUp()
    {
      _tickEvent = ScriptableObject.CreateInstance<ScriptableEventUint>();
      _commandEvent = ScriptableObject.CreateInstance<ScriptableEventICommand>();

      _pipelineObject = new GameObject("Pipeline_Test");
      _pipeline = _pipelineObject.AddComponent<LocalExecutionPipeline>();
      _pipeline.TickDispatchedEvent = _tickEvent;
      _pipeline.CommandSubmittedEvent = _commandEvent;
      _pipeline.Initialize(MockSimulationContextFactory.CreateEmptyContext());

      _mockView = new MockTelemetryView();
      _presenter = new SimulationTelemetryPresenter(_mockView , _tickEvent , _commandEvent);
      _presenter.Enable();
    }

    [TearDown]
    public void TearDown()
    {
      _presenter?.Dispose();

      if(_pipelineObject != null) { Object.DestroyImmediate(_pipelineObject); }

      if(_tickEvent != null) { Object.DestroyImmediate(_tickEvent); }

      if(_commandEvent != null) { Object.DestroyImmediate(_commandEvent); }
    }

    [Test]
    public void StepSimulation_RaisesSoapTickEvent_AndUpdatesView()
    {
      _pipeline.StepSimulation();

      Assert.That(_mockView.DisplayedTick , Is.EqualTo(SimulationConstants.InitialTick + SimulationConstants.SingleTickStep));
    }

    [Test]
    public void EnqueueCommand_RaisesSoapCommandEvent_AndUpdatesView()
    {
      var command = new MockTestCommand();

      _pipeline.EnqueueCommand(command);

      Assert.That(_mockView.LastReceivedCommand , Is.SameAs(command));
    }
  }
}