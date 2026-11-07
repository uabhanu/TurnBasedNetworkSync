namespace Tests.Editor.Reconciliation
{
  using Architecture.Contracts;
  using NUnit.Framework;
  using System;
  using System.Runtime.InteropServices;

  [TestFixture]
  public class SerializationPathwayProfilingTests
  {
    private const int BufferCapacityBytes = 32;
    private const long ExpectedAllocatedBytes = 0L;
    private const int ProfileIterations = 1000;
    private const byte TestStateKey = 10;
    private const byte TestTargetId = 1;
    private const uint TestTick = 42;
    private const int TestValue = 250;

    [Test]
    public void NetworkStateDelta_DirectMemoryBlit_ProducesZeroHeapAllocations()
    {
      GC.Collect();
      GC.WaitForPendingFinalizers();
      GC.Collect();

      var delta = new NetworkStateDelta(TestTick , TestTargetId , TestStateKey , TestValue);
      Span<byte> destinationSpan = stackalloc byte[BufferCapacityBytes];

      long memoryBefore = GC.GetAllocatedBytesForCurrentThread();

      for(var i = 0 ; i < ProfileIterations ; i++) MemoryMarshal.Write(destinationSpan , ref delta);

      long memoryAfter = GC.GetAllocatedBytesForCurrentThread();
      long totalAllocated = memoryAfter - memoryBefore;

      Assert.AreEqual(ExpectedAllocatedBytes , totalAllocated);
    }

    [Test]
    public void NetworkStateDelta_SpanDeserialization_ProducesZeroHeapAllocations()
    {
      GC.Collect();
      GC.WaitForPendingFinalizers();
      GC.Collect();

      Span<byte> sourceSpan = stackalloc byte[BufferCapacityBytes];
      var originalDelta = new NetworkStateDelta(TestTick , TestTargetId , TestStateKey , TestValue);
      MemoryMarshal.Write(sourceSpan , ref originalDelta);

      long memoryBefore = GC.GetAllocatedBytesForCurrentThread();

      for(var i = 0 ; i < ProfileIterations ; i++)
      {
        var restored = MemoryMarshal.Read<NetworkStateDelta>(sourceSpan);
        _ = restored.Tick;
      }

      long memoryAfter = GC.GetAllocatedBytesForCurrentThread();
      long totalAllocated = memoryAfter - memoryBefore;

      Assert.AreEqual(ExpectedAllocatedBytes , totalAllocated);
    }
  }
}