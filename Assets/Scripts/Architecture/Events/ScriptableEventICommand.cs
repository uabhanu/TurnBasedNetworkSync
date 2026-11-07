namespace Architecture.Events
{
  using Contracts;
  using Obvious.Soap;
  using UnityEngine;

  [CreateAssetMenu(fileName = "ScriptableEvent" + nameof(ICommand) , menuName = "Soap/ScriptableEvents/" + nameof(ICommand))]
  public class ScriptableEventICommand : ScriptableEvent<ICommand> {}
}