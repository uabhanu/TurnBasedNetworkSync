namespace Architecture.Core.Reconciliation
{
  using Contracts;
  using System;

  /// <summary>
  /// Stateless execution reversal service restoring past simulation frames from a snapshot buffer without heap allocations.
  /// </summary>
  public sealed class ExecutionReversalRoutine : IExecutionReversalRoutine
  {
    public static readonly ExecutionReversalRoutine Instance = new ExecutionReversalRoutine();

    public bool TryRevertToTick(uint targetTick , IStateSnapshotBuffer snapshotBuffer , out StateSnapshotFrame restoredFrame)
    {
      if(snapshotBuffer == null) { throw new ArgumentNullException(nameof(snapshotBuffer)); }

      if(snapshotBuffer.Count == 0 || targetTick > snapshotBuffer.LatestTick)
      {
        restoredFrame = default;
        return false;
      }

      return snapshotBuffer.TryGetSnapshot(targetTick , out restoredFrame);
    }
  }
}