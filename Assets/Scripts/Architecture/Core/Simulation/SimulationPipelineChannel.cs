namespace Architecture.Core.Simulation
{
  using Contracts;
  using System;
  using UnityEngine;

  [CreateAssetMenu(fileName = "SimulationPipelineChannel" , menuName = "TurnBasedSync/Architecture/Simulation Pipeline Channel")]
  public class SimulationPipelineChannel : ScriptableObject
  {
    public event Action<ICommand> CommandSubmitted;
    public event Action<uint> TickDispatched;

    public void RaiseCommandSubmitted(ICommand command) { CommandSubmitted?.Invoke(command); }

    public void RaiseTickDispatched(uint tick) { TickDispatched?.Invoke(tick); }
  }
}