namespace Architecture.Events
{
  using Obvious.Soap;
  using System;
  using UnityEngine;
  using UnityEngine.Events;

  [AddComponentMenu("Soap/EventListeners/EventListeneruint")]
  public class EventListenerUint : EventListenerGeneric<uint>
  {
    protected override EventResponse<uint>[] EventResponses => eventResponses;
    
    [SerializeField] private EventResponse[] eventResponses;

    [Serializable]
    public class EventResponse : EventResponse<uint>
    {
      [SerializeField] private ScriptableEventUint scriptableEvent;
      [SerializeField] private UintUnityEvent response;
      public override ScriptableEvent<uint> ScriptableEvent => scriptableEvent;
      public override UnityEvent<uint> Response => response;
    }

    [Serializable]
    public class UintUnityEvent : UnityEvent<uint> {}
  }
}