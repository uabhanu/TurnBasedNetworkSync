using System.Collections.Generic;
using Architecture.Contracts;
using Architecture.Core.Reconciliation;
using NUnit.Framework;

namespace Tests.Editor.Reconciliation
{
  [TestFixture]
  public class CommandSortingTests
  {
    private class TestCommand : ICommand
    {
      public CommandTypeId CommandTypeId { get; set; } = default;
      public byte PlayerId{get; set;}
      public int TickId{get; set;}
      public long Timestamp{get; set;}
      public int SequenceTag{get; set;}

      public void Execute(IGameStateContext context) {}
      public byte[] Serialize() => System.Array.Empty<byte>();
    }

    [Test]
    public void Sort_OrdersByTickId_Ascending()
    {
      var cmdLate = new TestCommand { TickId = 10 , Timestamp = 100 , PlayerId = 1 };
      var cmdEarly = new TestCommand { TickId = 2 , Timestamp = 500 , PlayerId = 2 };

      var list = new List<ICommand> { cmdLate , cmdEarly };
      CommandSorter.SortInPlace(list);

      Assert.AreEqual(2 , list[0].TickId);
      Assert.AreEqual(10 , list[1].TickId);
    }

    [Test]
    public void Sort_SameTick_OrdersByNetworkTimestamp_Ascending()
    {
      var cmdLateNetwork = new TestCommand { TickId = 5 , Timestamp = 2000L , PlayerId = 1 };
      var cmdEarlyNetwork = new TestCommand { TickId = 5 , Timestamp = 1000L , PlayerId = 2 };

      var list = new List<ICommand> { cmdLateNetwork , cmdEarlyNetwork };
      CommandSorter.SortInPlace(list);

      Assert.AreEqual(1000L , list[0].Timestamp);
      Assert.AreEqual(2000L , list[1].Timestamp);
    }

    [Test]
    public void Sort_SameTickAndTimestamp_OrdersBySequenceTag_Ascending()
    {
      var cmdHighTag = new TestCommand { TickId = 5 , Timestamp = 1000L , SequenceTag = 99 , PlayerId = 1 };
      var cmdLowTag = new TestCommand { TickId = 5 , Timestamp = 1000L , SequenceTag = 10 , PlayerId = 2 };

      var list = new List<ICommand> { cmdHighTag , cmdLowTag };
      CommandSorter.SortInPlace(list);

      Assert.AreEqual(10 , list[0].SequenceTag);
      Assert.AreEqual(99 , list[1].SequenceTag);
    }

    [Test]
    public void Sort_SameTickTimestampAndSequenceTag_ResolvesByPlayerId_TieBreaker()
    {
      var cmdPlayer2 = new TestCommand { TickId = 5 , Timestamp = 1000L , SequenceTag = 1 , PlayerId = 2 };
      var cmdPlayer1 = new TestCommand { TickId = 5 , Timestamp = 1000L , SequenceTag = 1 , PlayerId = 1 };

      var list = new List<ICommand> { cmdPlayer2 , cmdPlayer1 };
      CommandSorter.SortInPlace(list);

      Assert.AreEqual(1 , list[0].PlayerId);
      Assert.AreEqual(2 , list[1].PlayerId);
    }
  }
}