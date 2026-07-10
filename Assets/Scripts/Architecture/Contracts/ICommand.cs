namespace Architecture.Contracts
{
  public interface IGameStateContext {}

  public interface ICommand
  {
    byte CommandTypeId{get;}
    byte PlayerId{get;}
    int TickId{get;}

    void Execute(IGameStateContext context);
    byte[] Serialize();
  }
}