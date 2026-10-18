namespace Architecture.Contracts
{
  public enum CommandTypeId : byte
  {
    None = 0 ,
    MockIncrement = 1 ,
    TokenPlacement = 2 ,
    EndTurn = 3
  }
}