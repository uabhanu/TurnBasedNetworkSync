namespace Architecture.Events
{
  using Contracts;
  using Obvious.Soap;
  using UnityEngine;
  using UnityEngine.Events;

  [AddComponentMenu("Soap/EventListeners/EventListener" + nameof(ICommand))]
  public class EventListenerICommand : EventListenerGeneric<ICommand>
  {
    protected override EventResponse<ICommand>[] EventResponses => eventResponses;
    
    [SerializeField] private EventResponse[] eventResponses;

    [System.Serializable]
    public class EventResponse : EventResponse<ICommand>
    {
      [SerializeField] private CommandUnityEvent response;
      [SerializeField] private ScriptableEventICommand scriptableEvent;
      
      public override ScriptableEvent<ICommand> ScriptableEvent => scriptableEvent;
      public override UnityEvent<ICommand> Response => response;
    }

    [System.Serializable]
    public class CommandUnityEvent : UnityEvent<ICommand> {}
  }
}