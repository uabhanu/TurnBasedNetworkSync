namespace Runtime.Simulation
{
  using Architecture.Contracts;
  using System;

  public sealed class NetworkLatencySimulatorWrapper
  {
    private readonly struct DelayedCommandEntry
    {
      public readonly ICommand Command;
      public readonly long DeliverAtTimestamp;

      public DelayedCommandEntry(ICommand command , long deliverAtTimestamp)
      {
        Command = command;
        DeliverAtTimestamp = deliverAtTimestamp;
      }
    }

    private readonly DelayedCommandEntry[] _ringBuffer;
    private readonly int _bufferCapacity;
    private readonly long _latencyMilliseconds;
    private readonly long _packetDropIntervalMilliseconds;

    private int _headIndex;
    private int _tailIndex;
    private int _queuedCount;
    private long _lastDropTimestamp;

    public NetworkLatencySimulatorWrapper(int bufferCapacity , long latencyMilliseconds , long packetDropIntervalMilliseconds)
    {
      if(bufferCapacity <= 0) throw new ArgumentOutOfRangeException(nameof(bufferCapacity));

      _bufferCapacity = bufferCapacity;
      _latencyMilliseconds = latencyMilliseconds;
      _packetDropIntervalMilliseconds = packetDropIntervalMilliseconds;

      _ringBuffer = new DelayedCommandEntry[bufferCapacity];
      _headIndex = 0;
      _tailIndex = 0;
      _queuedCount = 0;
      _lastDropTimestamp = 0L;
    }

    public int QueuedCount => _queuedCount;

    public bool EnqueuePacket(ICommand command , long currentTimestamp)
    {
      if(command == null) throw new ArgumentNullException(nameof(command));

      // Check packet drop interval condition
      if(_packetDropIntervalMilliseconds > 0L)
        if(_lastDropTimestamp == 0L || currentTimestamp - _lastDropTimestamp >= _packetDropIntervalMilliseconds)
        {
          _lastDropTimestamp = currentTimestamp;
          return false; // Packet intentionally dropped
        }

      if(_queuedCount >= _bufferCapacity) return false; // Buffer overflow safety: drop packet

      long deliverAt = currentTimestamp + _latencyMilliseconds;
      _ringBuffer[_tailIndex] = new DelayedCommandEntry(command , deliverAt);
      _tailIndex = (_tailIndex + 1) % _bufferCapacity;
      _queuedCount++;

      return true;
    }

    public bool TryDequeueReadyPacket(long currentTimestamp , out ICommand readyCommand)
    {
      if(_queuedCount == 0)
      {
        readyCommand = null;
        return false;
      }

      ref readonly DelayedCommandEntry entry = ref _ringBuffer[_headIndex];
      if(currentTimestamp >= entry.DeliverAtTimestamp)
      {
        readyCommand = entry.Command;
        _ringBuffer[_headIndex] = default;
        _headIndex = (_headIndex + 1) % _bufferCapacity;
        _queuedCount--;
        return true;
      }

      readyCommand = null;
      return false;
    }

    public void Clear()
    {
      Array.Clear(_ringBuffer , 0 , _bufferCapacity);
      _headIndex = 0;
      _tailIndex = 0;
      _queuedCount = 0;
      _lastDropTimestamp = 0L;
    }
  }
}