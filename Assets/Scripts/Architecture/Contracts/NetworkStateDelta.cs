// Summary
// Represents an immutable, zero-allocation network state delta for deterministic simulation steps.
// Explicit layout guarantees a consistent, predictable byte size across network serialization boundaries.

namespace Architecture.Contracts
{
  using System;
  using System.Runtime.InteropServices;
  
  [StructLayout(LayoutKind.Sequential , Pack = 1)]
  public readonly struct NetworkStateDelta : IEquatable<NetworkStateDelta>
  {
    public uint Tick{get;}
    public byte TargetId{get;}
    public byte StateKey{get;}
    public int Value{get;}

    public NetworkStateDelta(uint tick , byte targetId , byte stateKey , int value)
    {
      Tick = tick;
      TargetId = targetId;
      StateKey = stateKey;
      Value = value;
    }

    public bool Equals(NetworkStateDelta other) { return Tick == other.Tick && TargetId == other.TargetId && StateKey == other.StateKey && Value == other.Value; }

    public override bool Equals(object obj) { return obj is NetworkStateDelta other && Equals(other); }

    public override int GetHashCode()
    {
      unchecked
      {
        var hash = (int)2166136261;
        hash = (hash * 16777619) ^ (int)Tick;
        hash = (hash * 16777619) ^ TargetId.GetHashCode();
        hash = (hash * 16777619) ^ StateKey.GetHashCode();
        hash = (hash * 16777619) ^ Value.GetHashCode();
        return hash;
      }
    }

    public static bool operator ==(NetworkStateDelta left , NetworkStateDelta right) { return left.Equals(right); }

    public static bool operator !=(NetworkStateDelta left , NetworkStateDelta right) { return !left.Equals(right); }

    public override string ToString() { return $"[Delta Tick:{Tick} Target:{TargetId} Key:{StateKey} Val:{Value}]"; }
  }
}