namespace Tests.Editor.Simulation
{
  using Architecture.Contracts;
  using Architecture.Core.Simulation;
  using NUnit.Framework;
  using UnityEngine;

  [TestFixture]
  public class SimulationPipelineListenerTests
  {
    private const uint TestTick = SimulationConstants.InitialTick + SimulationConstants.SingleTickStep;

    private GameObject _listenerObject;
    private SimulationPipelineListener _listener;
    private SimulationPipelineChannel _channel;

    private class MockTestCommand : ICommand
    {
      public CommandTypeId CommandTypeId{get;} = default;
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
      _channel = ScriptableObject.CreateInstance<SimulationPipelineChannel>();
      _listenerObject = new GameObject("Test_Listener");
      _listener = _listenerObject.AddComponent<SimulationPipelineListener>();
      _listener.Channel = _channel;
    }

    [TearDown]
    public void TearDown()
    {
      Object.DestroyImmediate(_listenerObject);
      Object.DestroyImmediate(_channel);
    }

    [Test]
    public void TickDispatched_PropagatesToListener_WhenSubscribed()
    {
      uint receivedTick = SimulationConstants.InitialTick;
      _channel.TickDispatched += tick => receivedTick = tick;

      _channel.RaiseTickDispatched(TestTick);

      Assert.That(receivedTick , Is.EqualTo(TestTick));
    }

    [Test]
    public void CommandSubmitted_PropagatesToListener_WhenSubscribed()
    {
      ICommand receivedCommand = null;
      _channel.CommandSubmitted += cmd => receivedCommand = cmd;
      var command = new MockTestCommand();

      _channel.RaiseCommandSubmitted(command);

      Assert.That(receivedCommand , Is.SameAs(command));
    }

    [Test]
    public void UnsubscribedListener_DoesNotReceiveChannelEvents()
    {
      uint receivedTick = SimulationConstants.InitialTick;
      _listener.enabled = false;

      _channel.RaiseTickDispatched(TestTick);

      Assert.That(receivedTick , Is.EqualTo(SimulationConstants.InitialTick));
    }
  }
}