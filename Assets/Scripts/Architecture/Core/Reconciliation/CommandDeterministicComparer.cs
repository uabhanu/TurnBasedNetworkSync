using System.Collections.Generic;
using Architecture.Contracts;

namespace Architecture.Core.Reconciliation
{
  /// <summary>
  /// Enforces Marmalade Studios deterministic command ordering:
  /// 1. Target Tick (Ascending)
  /// 2. Network Timestamp (Ascending)
  /// 3. Sequence Type Tag (Ascending)
  /// 4. Player/Client ID (Ascending - absolute tie breaker)
  /// </summary>
  public sealed class CommandDeterministicComparer : IComparer<ICommand>
  {
    public static readonly CommandDeterministicComparer Instance = new CommandDeterministicComparer();

    public int Compare(ICommand x , ICommand y)
    {
      if(ReferenceEquals(x , y)) return 0;
      if(x is null) return -1;
      if(y is null) return 1;

      // 1. TickId
      int tickComparison = x.TickId.CompareTo(y.TickId);
      if(tickComparison != 0) return tickComparison;

      // 2. Network Timestamp
      int timestampComparison = x.Timestamp.CompareTo(y.Timestamp);
      if(timestampComparison != 0) return timestampComparison;

      // 3. Sequence Type Tag
      int sequenceTagComparison = x.SequenceTag.CompareTo(y.SequenceTag);
      if(sequenceTagComparison != 0) return sequenceTagComparison;

      // 4. Deterministic Tie-Breaker: PlayerId
      return x.PlayerId.CompareTo(y.PlayerId);
    }
  }
}