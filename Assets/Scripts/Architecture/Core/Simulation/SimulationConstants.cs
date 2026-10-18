namespace Runtime.Core.Simulation
{
  /// <summary>
  /// Global architectural constraints and limits for the deterministic simulation layer.
  /// </summary>
  public static class SimulationConstants
  {
    public const uint InitialTick = 0;
    public const int MaxSectors = 16;
    public const int MaxNodesPerSector = 64;
    public const int DefaultEntityCapacity = 64;

    public const ushort EmptyNodeOccupantId = 0;
    public const byte DefaultInitialPlayerId = 1;
    public const byte SecondaryPlayerId = 2;
  }
}