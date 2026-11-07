namespace Tests.Tests
{
  using Architecture.Contracts;
  using NUnit.Framework;

  [TestFixture]
  public class NetworkStateDeltaTests
  {
    [Test]
    public void Constructor_InitializesFieldsCorrectly()
    {
      var delta = new NetworkStateDelta(tick : 42 , targetId : 3 , stateKey : 7 , value : 1050);

      Assert.That(delta.Tick , Is.EqualTo(42u));
      Assert.That(delta.TargetId , Is.EqualTo((byte)3));
      Assert.That(delta.StateKey , Is.EqualTo((byte)7));
      Assert.That(delta.Value , Is.EqualTo(1050));
    }

    [Test]
    public void Equality_IdenticalDeltas_AreEqual()
    {
      var deltaA = new NetworkStateDelta(10 , 1 , 2 , 100);
      var deltaB = new NetworkStateDelta(10 , 1 , 2 , 100);

      Assert.That(deltaA , Is.EqualTo(deltaB));
      Assert.That(deltaA == deltaB , Is.True);
      Assert.That(deltaA.GetHashCode() , Is.EqualTo(deltaB.GetHashCode()));
    }

    [Test]
    public void Inequality_DifferentProperties_AreNotEqual()
    {
      var baseDelta = new NetworkStateDelta(10 , 1 , 2 , 100);
      var diffTick = new NetworkStateDelta(11 , 1 , 2 , 100);
      var diffTarget = new NetworkStateDelta(10 , 2 , 2 , 100);
      var diffKey = new NetworkStateDelta(10 , 1 , 3 , 100);
      var diffVal = new NetworkStateDelta(10 , 1 , 2 , 999);

      Assert.That(baseDelta == diffTick , Is.False);
      Assert.That(baseDelta == diffTarget , Is.False);
      Assert.That(baseDelta == diffKey , Is.False);
      Assert.That(baseDelta == diffVal , Is.False);
    }
  }
}