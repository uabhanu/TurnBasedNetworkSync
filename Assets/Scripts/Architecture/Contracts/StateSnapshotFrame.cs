namespace Architecture.Contracts
{
  using System;
  using System.Runtime.InteropServices;

  /// <summary>
  ///   Represents an immutable structural state snapshot frame captured at a given simulation tick.
  /// </summary>
  [StructLayout(LayoutKind.Sequential , Pack = 1)]
  public readonly struct StateSnapshotFrame : IEquatable<StateSnapshotFrame>
  {
    public NetworkStateDelta Delta{get;}
    public uint Tick{get;}
    public long Timestamp{get;}

    public StateSnapshotFrame(NetworkStateDelta delta , uint tick , long timestamp)
    {
      Delta = delta;
      Tick = tick;
      Timestamp = timestamp;
    }

    public bool Equals(StateSnapshotFrame other) { return Tick == other.Tick && Delta.Equals(other.Delta) && Timestamp == other.Timestamp; }

    public override bool Equals(object obj) { return obj is StateSnapshotFrame other && Equals(other); }

    public override int GetHashCode()
    {
      unchecked
      {
        var hash = 17;
        hash = (hash * 31) ^ (int)Tick;
        hash = (hash * 31) ^ Delta.GetHashCode();
        hash = (hash * 31) ^ Timestamp.GetHashCode();
        return hash;
      }
    }

    public static bool operator ==(StateSnapshotFrame left , StateSnapshotFrame right) { return left.Equals(right); }

    public static bool operator !=(StateSnapshotFrame left , StateSnapshotFrame right) { return !left.Equals(right); }
  }
}