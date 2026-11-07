namespace Tests.Editor.Reconciliation
{
  using System;
  using Architecture.Contracts;
  using Architecture.Core.Reconciliation;
  using NUnit.Framework;

  [TestFixture]
  public class ExecutionReversalRoutineTests
  {
    private const int BufferCapacity = 16;
    private const uint FutureTick = 10;
    private const byte TestStateKey = 2;
    private const long TestTimestamp = 2500L;
    private const byte TestTargetId = 1;
    private const int TestValue = 42;
    private const uint ValidTick = 5;

    private StateSnapshotBuffer _buffer;
    private ExecutionReversalRoutine _routine;

    [SetUp]
    public void SetUp()
    {
      _buffer = new StateSnapshotBuffer(BufferCapacity);
      _routine = ExecutionReversalRoutine.Instance;
    }

    [Test]
    public void TryRevertToTick_ThrowsArgumentNullException_WhenBufferIsNull() { Assert.Throws<ArgumentNullException>(() => { _routine.TryRevertToTick(ValidTick , null , out _); }); }

    [Test]
    public void TryRevertToTick_ReturnsFalse_WhenBufferIsEmpty()
    {
      bool success = _routine.TryRevertToTick(ValidTick , _buffer , out StateSnapshotFrame restoredFrame);

      Assert.IsFalse(success);
      Assert.AreEqual(default(StateSnapshotFrame) , restoredFrame);
    }

    [Test]
    public void TryRevertToTick_ReturnsFalse_WhenTargetTickIsInTheFuture()
    {
      var delta = new NetworkStateDelta(ValidTick , TestTargetId , TestStateKey , TestValue);
      _buffer.RecordSnapshot(ValidTick , delta , TestTimestamp);

      bool success = _routine.TryRevertToTick(FutureTick , _buffer , out StateSnapshotFrame restoredFrame);

      Assert.IsFalse(success);
      Assert.AreEqual(default(StateSnapshotFrame) , restoredFrame);
    }

    [Test]
    public void TryRevertToTick_RestoresFrameSuccessfully_WhenSnapshotExists()
    {
      var delta = new NetworkStateDelta(ValidTick , TestTargetId , TestStateKey , TestValue);
      _buffer.RecordSnapshot(ValidTick , delta , TestTimestamp);

      bool success = _routine.TryRevertToTick(ValidTick , _buffer , out StateSnapshotFrame restoredFrame);

      Assert.IsTrue(success);
      Assert.AreEqual(ValidTick , restoredFrame.Tick);
      Assert.AreEqual(delta , restoredFrame.Delta);
      Assert.AreEqual(TestTimestamp , restoredFrame.Timestamp);
    }
  }
}