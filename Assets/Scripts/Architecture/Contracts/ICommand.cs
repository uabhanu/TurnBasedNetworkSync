namespace Architecture.Contracts
{
  public interface ICommand
  {
    CommandTypeId CommandTypeId{get;}
    byte PlayerId{get;}
    int SequenceTag { get; }
    int TickId{get;}
    long Timestamp { get; }

    void Execute(IGameStateContext context);
    byte[] Serialize();
  }
}