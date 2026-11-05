namespace Tests.Editor.Reconciliation
{
  using System;
  using System.Collections.Generic;
  using Architecture.Contracts;
  using Architecture.Core.Reconciliation;
  using NUnit.Framework;

  [TestFixture]
  public class HighFrequencyOutOfOrderExecutionTests
  {
    private const long BaseTimestamp = 1000L;
    private const int BatchSize = 16;
    private const uint ExpiredTick = 2;
    private const int InitialBaseValue = 100;
    private const uint InitialSimulationTick = 1;
    private const uint LateArrivingPacketTick = 4;
    private const int PlayerModuloDivisor = 2;
    private const int RandomSeed = 1337;
    private const int SnapshotCapacity = 64;
    private const byte TestStateKey = 10;
    private const byte TestTargetId = 1;
    private const long TimestampStep = 50L;
    private const int WrapLimitOffset = 10;

    private ExecutionReversalRoutine _reversalRoutine;
    private StateSnapshotBuffer _snapshotBuffer;

    private class HighFrequencyTestCommand : ICommand
    {
      public CommandTypeId CommandTypeId{get; set;} = default;
      public byte PlayerId{get; set;}
      public int TickId{get; set;}
      public long Timestamp{get; set;}
      public int SequenceTag{get; set;}

      public void Execute(IGameStateContext context) {}

      public byte[] Serialize() => Array.Empty<byte>();
    }

    [SetUp]
    public void SetUp()
    {
      _snapshotBuffer = new StateSnapshotBuffer(SnapshotCapacity);
      _reversalRoutine = ExecutionReversalRoutine.Instance;
    }

    [Test]
    public void HighFrequencyStream_SortsShuffledCommandsInStrictDeterministicOrder()
    {
      var unsortedCommands = new List<ICommand>(BatchSize);
      var rng = new Random(RandomSeed);

      for(int i = 0 ; i < BatchSize ; i++) { unsortedCommands.Add(new HighFrequencyTestCommand { TickId = (int)InitialSimulationTick + i , Timestamp = BaseTimestamp + (i * TimestampStep) , SequenceTag = BatchSize - i , PlayerId = (byte)(i % PlayerModuloDivisor) }); }

      // Shuffle list using Fisher-Yates
      for(int i = BatchSize - 1 ; i > 0 ; i--)
      {
        int swapIndex = rng.Next(i + 1);
        (unsortedCommands[i] , unsortedCommands[swapIndex]) = (unsortedCommands[swapIndex] , unsortedCommands[i]);
      }

      CommandSorter.SortInPlace(unsortedCommands);

      for(int i = 0 ; i < BatchSize - 1 ; i++)
      {
        Assert.LessOrEqual(unsortedCommands[i].TickId , unsortedCommands[i + 1].TickId);

        if(unsortedCommands[i].TickId == unsortedCommands[i + 1].TickId) { Assert.LessOrEqual(unsortedCommands[i].Timestamp , unsortedCommands[i + 1].Timestamp); }
      }
    }

    [Test]
    public void HighFrequencyArrival_LateArrivalTriggersSuccessfulStateReversal()
    {
      for(uint tick = InitialSimulationTick ; tick <= BatchSize ; tick++)
      {
        var delta = new NetworkStateDelta(tick , TestTargetId , TestStateKey , InitialBaseValue + (int)tick);
        _snapshotBuffer.RecordSnapshot(tick , delta , BaseTimestamp + (tick * TimestampStep));
      }

      bool reversed = _reversalRoutine.TryRevertToTick(LateArrivingPacketTick , _snapshotBuffer , out StateSnapshotFrame restoredFrame);

      Assert.IsTrue(reversed);
      Assert.AreEqual(LateArrivingPacketTick , restoredFrame.Tick);
      Assert.AreEqual(InitialBaseValue + (int)LateArrivingPacketTick , restoredFrame.Delta.Value);
    }

    [Test]
    public void HighFrequencyRingWrap_RollbackToOverwrittenTickSafelyFails()
    {
      const int wrapLimit = SnapshotCapacity + WrapLimitOffset;

      for(uint tick = InitialSimulationTick ; tick <= wrapLimit ; tick++)
      {
        var delta = new NetworkStateDelta(tick , TestTargetId , TestStateKey , (int)tick);
        _snapshotBuffer.RecordSnapshot(tick , delta , BaseTimestamp + tick);
      }

      bool reversed = _reversalRoutine.TryRevertToTick(ExpiredTick , _snapshotBuffer , out StateSnapshotFrame restoredFrame);

      Assert.IsFalse(reversed);
      Assert.AreEqual(default(StateSnapshotFrame) , restoredFrame);
    }
  }
}