namespace Architecture.Contracts
{
  /// <summary>
  /// Contract for zero-allocation circular state snapshot buffer routines used during rollback reconciliation.
  /// </summary>
  public interface IStateSnapshotBuffer
  {
    int Capacity{get;}
    int Count{get;}
    uint LatestTick{get;}

    void Clear();
    void RecordSnapshot(uint tick , NetworkStateDelta delta , long timestamp);
    bool TryGetSnapshot(uint tick , out StateSnapshotFrame frame);
  }
}