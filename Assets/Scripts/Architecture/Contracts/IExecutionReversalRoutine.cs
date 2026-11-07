namespace Architecture.Contracts
{
  /// <summary>
  /// Contract for executing zero-allocation state rollbacks to past simulation ticks upon late packet detection.
  /// </summary>
  public interface IExecutionReversalRoutine
  {
    bool TryRevertToTick(uint targetTick , IStateSnapshotBuffer snapshotBuffer , out StateSnapshotFrame restoredFrame);
  }
}