using System;
using System.Collections.Generic;
using Architecture.Contracts;

namespace Architecture.Core.Reconciliation
{
  public static class CommandSorter
  {
    public static void SortInPlace(List<ICommand> commands)
    {
      if(commands == null) throw new ArgumentNullException(nameof(commands));
      commands.Sort(CommandDeterministicComparer.Instance);
    }

    public static void SortInPlace(ICommand[] commands)
    {
      if(commands == null) throw new ArgumentNullException(nameof(commands));
      Array.Sort(commands , CommandDeterministicComparer.Instance);
    }
  }
}