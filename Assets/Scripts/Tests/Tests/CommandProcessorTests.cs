namespace Tests
{
  using System;
  using Architecture;
  using Architecture.Contracts;
  using NUnit.Framework;

  public class CommandProcessorTests
  {
    private const byte DefaultPlayerId = 0;
    private const int DefaultTickId = 1;
    private const int InitialContextValue = 0;

    private class MockGameStateContext : IGameStateContext
    {
      public int Value{get; set;}
    }

    private class MockIncrementCommand : ICommand
    {
      public MockIncrementCommand(byte playerId = DefaultPlayerId , int tickId = DefaultTickId)
      {
        PlayerId = playerId;
        TickId = tickId;
      }

      public CommandTypeId CommandTypeId => CommandTypeId.MockIncrement;
      public byte PlayerId{get;}
      public int TickId{get;}

      public void Execute(IGameStateContext context)
      {
        if(context is MockGameStateContext mockContext) { mockContext.Value++; }
      }

      public byte[] Serialize() => Array.Empty<byte>();
    }

    [Test]
    public void ProcessNext_ExecutesQueuedCommand_AndDecrementsCount()
    {
      var context = new MockGameStateContext { Value = InitialContextValue };
      var processor = new CommandProcessor(context);
      var command = new MockIncrementCommand();

      processor.Enqueue(command);
      Assert.AreEqual(1 , processor.QueuedCount);

      bool executed = processor.ProcessNext();

      Assert.IsTrue(executed);
      Assert.AreEqual(0 , processor.QueuedCount);
      Assert.AreEqual(InitialContextValue + 1 , context.Value);
    }

    [Test]
    public void ProcessAll_ExecutesAllQueuedCommands()
    {
      const int commandBatchSize = 3;
      var context = new MockGameStateContext { Value = InitialContextValue };
      var processor = new CommandProcessor(context);

      for(int i = 0 ; i < commandBatchSize ; i++) { processor.Enqueue(new MockIncrementCommand()); }

      int processed = processor.ProcessAll();

      Assert.AreEqual(commandBatchSize , processed);
      Assert.AreEqual(0 , processor.QueuedCount);
      Assert.AreEqual(InitialContextValue + commandBatchSize , context.Value);
    }
  }
}