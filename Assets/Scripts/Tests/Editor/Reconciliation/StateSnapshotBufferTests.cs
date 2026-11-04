namespace Tests.Editor.Reconciliation
{
  using Architecture.Contracts;
  using Architecture.Core.Reconciliation;
  using NUnit.Framework;

  [TestFixture]
  public class StateSnapshotBufferTests
  {
    private const int BufferCapacity = 16;
    private const uint NonExistentTick = 999;
    private const int OverflowFrameCount = 5;
    private const uint OverwrittenTick = 0;
    private const byte TestStateKey = 2;
    private const byte TestTargetId = 1;
    private const uint TestTick = 1;
    private const long TestTimestamp = 1000L;
    private const int TestValue = 100;

    private StateSnapshotBuffer _buffer;

    [SetUp]
    public void SetUp() { _buffer = new StateSnapshotBuffer(BufferCapacity); }

    [Test]
    public void RecordSnapshot_StoresFrame_AndIncrementsCount()
    {
      var delta = new NetworkStateDelta(TestTick , TestTargetId , TestStateKey , TestValue);

      _buffer.RecordSnapshot(TestTick , delta , TestTimestamp);

      Assert.AreEqual(1 , _buffer.Count);
      Assert.AreEqual(TestTick , _buffer.LatestTick);
      Assert.IsTrue(_buffer.TryGetSnapshot(TestTick , out StateSnapshotFrame frame));
      Assert.AreEqual(TestTick , frame.Tick);
      Assert.AreEqual(delta , frame.Delta);
      Assert.AreEqual(TestTimestamp , frame.Timestamp);
    }

    [Test]
    public void TryGetSnapshot_NonExistentTick_ReturnsFalse() { Assert.IsFalse(_buffer.TryGetSnapshot(NonExistentTick , out _)); }

    [Test]
    public void RecordSnapshot_OverwritesOldFrames_WhenExceedingCapacity()
    {
      const int totalFramesToRecord = BufferCapacity + OverflowFrameCount;

      for(uint i = 0 ; i < totalFramesToRecord ; i++)
      {
        var delta = new NetworkStateDelta(i , TestTargetId , TestStateKey , (int)i);
        _buffer.RecordSnapshot(i , delta , TestTimestamp);
      }

      Assert.AreEqual(BufferCapacity , _buffer.Count);
      Assert.AreEqual(totalFramesToRecord - 1 , _buffer.LatestTick);

      Assert.IsFalse(_buffer.TryGetSnapshot(OverwrittenTick , out _));
      Assert.IsTrue(_buffer.TryGetSnapshot(BufferCapacity , out StateSnapshotFrame frame));
      
      Assert.AreEqual(BufferCapacity , frame.Tick);
    }

    [Test]
    public void Clear_ResetsCountAndLatestTick()
    {
      var delta = new NetworkStateDelta(TestTick , TestTargetId , TestStateKey , TestValue);
      _buffer.RecordSnapshot(TestTick , delta , TestTimestamp);

      _buffer.Clear();

      Assert.AreEqual(0 , _buffer.Count);
      Assert.AreEqual(0 , _buffer.LatestTick);
      
      Assert.IsFalse(_buffer.TryGetSnapshot(TestTick , out _));
    }
  }
}