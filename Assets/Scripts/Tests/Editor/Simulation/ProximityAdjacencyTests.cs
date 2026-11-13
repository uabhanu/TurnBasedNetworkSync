namespace Tests.Editor.Simulation
{
  using NUnit.Framework;
  using Runtime.Commands;
  using Runtime.State;

  public class ProximityAdjacencyTests
  {
    private const uint InitialTick = 0u;
    private const uint FirstCommandId = 1u;
    private const uint SecondCommandId = 2u;
    private const uint ThirdCommandId = 3u;
    private const uint ExpectedTickAfterFirstMove = 1u;
    private const uint ExpectedTickAfterSecondMove = 2u;
    private const uint ExpectedTickAfterThirdMove = 3u;

    // Exact coordinates from the video (Row 3 & Row 4, Columns 4, 5, 6)
    private const int Row3 = 3;
    private const int Row4 = 4;
    private const int Col4 = 4;
    private const int Col5 = 5;
    private const int Col6 = 6;

    private const byte InitialCentreTileValue = 7;
    private const byte FriendlyBuffTileValue = 2;
    private const byte ExpectedBuffedValue = 8;
    private const ushort ExpectedScoreAfterBuff = 10;

    private const byte DiagonalCaptureTileValue = 16;
    private const ushort ExpectedP1ScoreAfterDiagonalLoss = 0;
    private const ushort ExpectedP2ScoreAfterDiagonalCapture = 26;

    private const byte MultiCaptureTileValue = 19;
    private const ushort ExpectedP1ScoreAfterMultiCapture = 43;
    private const ushort ExpectedP2ScoreAfterMultiLoss = 2;

    private const int TopLeftCellIndex = 0;
    private const byte AdjacentCellIndex = 1;
    private const byte EqualTieTileValue = 15;

    [Test]
    public void PlaceTileCommand_ResolvesOrthogonalAndDiagonalNeighbours_MatchingVideoSequence()
    {
      var state = new ProximityGameState { SimulationTick = InitialTick , ActivePlayerId = ProximityGameState.Player1Id };

      int cellR3C5 = Row3 * ProximityGameState.Columns + Col5;
      int cellR3C4 = Row3 * ProximityGameState.Columns + Col4;
      int cellR4C5 = Row4 * ProximityGameState.Columns + Col5;
      int cellR3C6 = Row3 * ProximityGameState.Columns + Col6;

      // Video 00:03 - Player 1 starts with 7 at (Col 5, Row 3)
      state.Owners[cellR3C5] = ProximityGameState.Player1Id;
      state.Values[cellR3C5] = InitialCentreTileValue;

      // Video 00:09 - Player 1 places 2 at (Col 4, Row 3) -> Buffs (Col 5, Row 3) from 7 to 8
      var buffCmd = new PlaceTileCommand { CommandId = FirstCommandId , TargetTick = InitialTick , PlayerId = ProximityGameState.Player1Id , CellIndex = (byte)cellR3C4 , TileValue = FriendlyBuffTileValue };
      buffCmd.Execute(ref state);

      Assert.AreEqual(ExpectedBuffedValue , state.Values[cellR3C5] , "Friendly neighbour should buff from 7 to 8");
      Assert.AreEqual(ExpectedScoreAfterBuff , state.ScoreP1 , "Player 1 score should be 8 + 2 = 10");
      Assert.AreEqual(ExpectedTickAfterFirstMove , state.SimulationTick);

      // Video 00:11 - Player 2 places 16 at (Col 5, Row 4) -> Captures BOTH (5,3) [North] and (4,3) [North-West Diagonal]
      var diagonalCaptureCmd = new PlaceTileCommand { CommandId = SecondCommandId , TargetTick = ExpectedTickAfterFirstMove , PlayerId = ProximityGameState.Player2Id , CellIndex = (byte)cellR4C5 , TileValue = DiagonalCaptureTileValue };
      diagonalCaptureCmd.Execute(ref state);

      Assert.AreEqual(ProximityGameState.Player2Id , state.Owners[cellR3C5] , "Orthogonal North neighbour (8) should be captured");
      Assert.AreEqual(ProximityGameState.Player2Id , state.Owners[cellR3C4] , "Diagonal North-West neighbour (2) should be captured");
      Assert.AreEqual(ExpectedP1ScoreAfterDiagonalLoss , state.ScoreP1);
      Assert.AreEqual(ExpectedP2ScoreAfterDiagonalCapture , state.ScoreP2 , "Player 2 score should be 16 + 8 + 2 = 26");
      Assert.AreEqual(ExpectedTickAfterSecondMove , state.SimulationTick);

      // Video 00:22 - Player 1 places 19 at (Col 6, Row 3) -> Captures (5,3) [West] and (5,4) [South-West Diagonal], while (4,3) stays Player 2
      var multiCaptureCmd = new PlaceTileCommand { CommandId = ThirdCommandId , TargetTick = ExpectedTickAfterSecondMove , PlayerId = ProximityGameState.Player1Id , CellIndex = (byte)cellR3C6 , TileValue = MultiCaptureTileValue };
      multiCaptureCmd.Execute(ref state);

      Assert.AreEqual(ProximityGameState.Player1Id , state.Owners[cellR3C5] , "West neighbour (8) should be captured by 19");
      Assert.AreEqual(ProximityGameState.Player1Id , state.Owners[cellR4C5] , "South-West diagonal neighbour (16) should be captured by 19");
      Assert.AreEqual(ProximityGameState.Player2Id , state.Owners[cellR3C4] , "Cell two columns away (2) must remain untouched");
      Assert.AreEqual(ExpectedP1ScoreAfterMultiCapture , state.ScoreP1 , "Player 1 score should be 19 + 16 + 8 = 43");
      Assert.AreEqual(ExpectedP2ScoreAfterMultiLoss , state.ScoreP2 , "Player 2 score should retain only the 2 tile");
      Assert.AreEqual(ExpectedTickAfterThirdMove , state.SimulationTick);
    }

    [Test]
    public void PlaceTileCommand_DoesNotCapture_WhenEnemyTileIsEqualOrHigher()
    {
      var state = new ProximityGameState { SimulationTick = InitialTick , ActivePlayerId = ProximityGameState.Player2Id };

      state.Owners[TopLeftCellIndex] = ProximityGameState.Player1Id;
      state.Values[TopLeftCellIndex] = EqualTieTileValue;

      var cmd = new PlaceTileCommand { CommandId = FirstCommandId , TargetTick = InitialTick , PlayerId = ProximityGameState.Player2Id , CellIndex = AdjacentCellIndex , TileValue = EqualTieTileValue };
      cmd.Execute(ref state);

      Assert.AreEqual(ProximityGameState.Player1Id , state.Owners[TopLeftCellIndex] , "Enemy tile of equal value must not be captured");
    }
  }
}