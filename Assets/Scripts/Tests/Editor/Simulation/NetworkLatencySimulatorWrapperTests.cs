namespace Tests.Editor.Simulation
{
  using Architecture.Contracts;
  using NUnit.Framework;
  using Runtime.Simulation;
  using System;

  [TestFixture]
  public class NetworkLatencySimulatorWrapperTests
  {
    private const int BufferCapacity = 32;
    private const long DisabledDropIntervalMilliseconds = 0L;
    private const int ExpectedZeroAllocatedBytes = 0;
    private const long LatencyMilliseconds = 50L;
    private const long PacketDropIntervalMilliseconds = 150L;
    private const long PreThresholdOffsetMilliseconds = 1L;
    private const int ProfileIterations = 1000;
    private const long StartTimestamp = 1000L;
    private const byte TestPlayerId = 1;
    private const int TestTickId = 10;

    private class LatencyTestCommand : ICommand
    {
      public CommandTypeId CommandTypeId{get; set;} = default;
      public byte PlayerId{get; set;}
      public int TickId{get; set;}
      public long Timestamp{get; set;}
      public int SequenceTag{get; set;}

      public void Execute(IGameStateContext context) {}

      public byte[] Serialize() { return Array.Empty<byte>(); }
    }

    private NetworkLatencySimulatorWrapper _wrapper;
    private LatencyTestCommand _testCommand;

    [SetUp]
    public void SetUp()
    {
      _wrapper = new NetworkLatencySimulatorWrapper(BufferCapacity , LatencyMilliseconds , PacketDropIntervalMilliseconds);

      _testCommand = new LatencyTestCommand { PlayerId = TestPlayerId , TickId = TestTickId , Timestamp = StartTimestamp };
    }

    [Test]
    public void EnqueuePacket_AtDropInterval_DropsPacketConsistently()
    {
      // First packet triggers the initial drop timestamp
      bool enqueuedFirst = _wrapper.EnqueuePacket(_testCommand , StartTimestamp);
      Assert.IsFalse(enqueuedFirst);

      // Packet before 150ms should succeed
      long beforeInterval = StartTimestamp + (PacketDropIntervalMilliseconds - PreThresholdOffsetMilliseconds);
      bool enqueuedIntermediate = _wrapper.EnqueuePacket(_testCommand , beforeInterval);
      Assert.IsTrue(enqueuedIntermediate);

      // Packet at exact 150ms drop interval boundary must drop
      long atInterval = StartTimestamp + PacketDropIntervalMilliseconds;
      bool enqueuedAtInterval = _wrapper.EnqueuePacket(_testCommand , atInterval);
      Assert.IsFalse(enqueuedAtInterval);
    }

    [Test]
    public void TryDequeueReadyPacket_DelaysPacketUntilLatencyElapsed()
    {
      // Allow enqueue by clearing drop intervals for this scenario
      var zeroDropWrapper = new NetworkLatencySimulatorWrapper(BufferCapacity , LatencyMilliseconds , DisabledDropIntervalMilliseconds);
      zeroDropWrapper.EnqueuePacket(_testCommand , StartTimestamp);

      // Attempt dequeue prior to latency elapsed
      long beforeLatency = StartTimestamp + (LatencyMilliseconds - PreThresholdOffsetMilliseconds);
      bool prematureResult = zeroDropWrapper.TryDequeueReadyPacket(beforeLatency , out ICommand prematureCommand);
      Assert.IsFalse(prematureResult);
      Assert.IsNull(prematureCommand);

      // Attempt dequeue at or after latency elapsed
      long readyTimestamp = StartTimestamp + LatencyMilliseconds;
      bool readyResult = zeroDropWrapper.TryDequeueReadyPacket(readyTimestamp , out ICommand readyCommand);
      Assert.IsTrue(readyResult);
      Assert.AreSame(_testCommand , readyCommand);
    }

    [Test]
    public void PipelineOperations_UnderSustainedLoop_ProduceZeroHeapAllocations()
    {
      var zeroDropWrapper = new NetworkLatencySimulatorWrapper(BufferCapacity , LatencyMilliseconds , DisabledDropIntervalMilliseconds);

      GC.Collect();
      GC.WaitForPendingFinalizers();
      GC.Collect();

      long memoryBefore = GC.GetAllocatedBytesForCurrentThread();

      for(var i = 0 ; i < ProfileIterations ; i++)
      {
        long currentTimestamp = StartTimestamp + i;
        zeroDropWrapper.EnqueuePacket(_testCommand , currentTimestamp);
        zeroDropWrapper.TryDequeueReadyPacket(currentTimestamp + LatencyMilliseconds , out _);
      }

      long memoryAfter = GC.GetAllocatedBytesForCurrentThread();
      long totalAllocated = memoryAfter - memoryBefore;

      Assert.AreEqual(ExpectedZeroAllocatedBytes , totalAllocated);
    }
  }
}