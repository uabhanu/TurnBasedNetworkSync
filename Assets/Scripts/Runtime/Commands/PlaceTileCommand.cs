namespace Runtime.Commands
{
  using State;
  using System;
  using System.Runtime.InteropServices;

  [StructLayout(LayoutKind.Sequential , Pack = 1)]
  public struct PlaceTileCommand : IEquatable<PlaceTileCommand>
  {
    private const int MinGridCoordinate = 0;
    private const int MinNeighbourOffset = -1;
    private const int MaxNeighbourOffset = 1;
    private const int SelfOffset = 0;

    public uint CommandId;
    public uint TargetTick;
    public byte PlayerId;
    public byte CellIndex;
    public byte TileValue;

    public void Execute(ref ProximityGameState state)
    {
      if(CellIndex >= ProximityGameState.TotalCells) return;
      if(TileValue < ProximityGameState.MinTileValue || TileValue > ProximityGameState.MaxTileValue) return;
      if(state.Owners[CellIndex] != ProximityGameState.EmptyOwner) return;

      // 1. Place the tile
      state.Owners[CellIndex] = PlayerId;
      state.Values[CellIndex] = TileValue;

      // 2. Resolve all 8 surrounding neighbours (orthogonal + diagonal)
      int x = CellIndex % ProximityGameState.Columns;
      int y = CellIndex / ProximityGameState.Columns;

      for(int dy = MinNeighbourOffset ; dy <= MaxNeighbourOffset ; dy++)
      {
        for(int dx = MinNeighbourOffset ; dx <= MaxNeighbourOffset ; dx++)
        {
          if(dx == SelfOffset && dy == SelfOffset) continue;
          ResolveNeighbour(ref state , x + dx , y + dy , PlayerId , TileValue);
        }
      }

      // 3. Update totals & advance turn
      state.RecalculateScores();
      state.ActivePlayerId = state.ActivePlayerId == ProximityGameState.Player1Id ? ProximityGameState.Player2Id : ProximityGameState.Player1Id;
      state.SimulationTick++;
    }

    private static void ResolveNeighbour(ref ProximityGameState state , int nx , int ny , byte placerId , byte placerValue)
    {
      if(nx < MinGridCoordinate || nx >= ProximityGameState.Columns || ny < MinGridCoordinate || ny >= ProximityGameState.Rows) { return; }

      int nIndex = ny * ProximityGameState.Columns + nx;
      byte nOwner = state.Owners[nIndex];

      if(nOwner == ProximityGameState.EmptyOwner) return;

      if(nOwner == placerId)
      {
        byte currentVal = state.Values[nIndex];
        if(currentVal < ProximityGameState.MaxCellValue) { state.Values[nIndex] = (byte)(currentVal + ProximityGameState.FriendlyBuffIncrement); }
      }
      else if(placerValue > state.Values[nIndex]) { state.Owners[nIndex] = placerId; }
    }

    public readonly bool Equals(PlaceTileCommand other) { return CommandId == other.CommandId && TargetTick == other.TargetTick && PlayerId == other.PlayerId && CellIndex == other.CellIndex && TileValue == other.TileValue; }

    public readonly override bool Equals(object obj) => obj is PlaceTileCommand other && Equals(other);

    public readonly override int GetHashCode() => HashCode.Combine(CommandId , TargetTick , PlayerId , CellIndex , TileValue);
  }
}