namespace Architecture.Core.Reconciliation
{
  using Contracts;
  using System;

  /// <summary>
  ///   High-performance, zero-allocation ring buffer snapshot logger for tracking structural rollback frames.
  /// </summary>
  public sealed class StateSnapshotBuffer : IStateSnapshotBuffer
  {
    private readonly StateSnapshotFrame[] _buffer;
    private readonly int _capacity;
    private int _count;
    private uint _latestTick;

    public int Capacity => _capacity;
    public int Count => _count;
    public uint LatestTick => _latestTick;

    public StateSnapshotBuffer(int capacity)
    {
      if(capacity <= 0) throw new ArgumentOutOfRangeException(nameof(capacity) , "Capacity must be greater than zero.");

      _capacity = capacity;
      _buffer = new StateSnapshotFrame[capacity];
      _count = 0;
      _latestTick = 0;
    }

    public void RecordSnapshot(uint tick , NetworkStateDelta delta , long timestamp)
    {
      var index = (int)(tick % (uint)_capacity);
      _buffer[index] = new StateSnapshotFrame(delta , tick , timestamp);

      if(_count < _capacity) _count++;

      if(tick > _latestTick) _latestTick = tick;
    }

    public bool TryGetSnapshot(uint tick , out StateSnapshotFrame frame)
    {
      var index = (int)(tick % (uint)_capacity);
      StateSnapshotFrame candidate = _buffer[index];

      if(candidate.Tick == tick && _count > 0)
      {
        frame = candidate;
        return true;
      }

      frame = default;
      return false;
    }

    public void Clear()
    {
      Array.Clear(_buffer , 0 , _capacity);
      _count = 0;
      _latestTick = 0;
    }
  }
}