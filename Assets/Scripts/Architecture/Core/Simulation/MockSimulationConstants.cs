namespace Runtime.Core.Simulation
{
  /// <summary>
  /// Fixture constants used specifically for mock setups and testing pipelines.
  /// </summary>
  public static class MockSimulationConstants
  {
    public const ushort PlayerOneEntityId = 101;
    public const byte PlayerOneSector = 0;
    public const byte PlayerOneNodeIndex = 1;

    public const ushort PlayerTwoEntityId = 201;
    public const byte PlayerTwoSector = 1;
    public const byte PlayerTwoNodeIndex = 5;

    public const ushort DefaultUnitHealth = 100;
    public const ushort DefaultUnitEnergy = 50;
  }
}