using System.Collections.Generic;
using PlumJsonAnimator.Common.Constants;

namespace PlumJsonAnimator.Models.Commands;

abstract class IHistory
{
    protected LinkedList<ICommand> commands = new LinkedList<ICommand>();
    private readonly int MAX_CAPACITY = Consts.MAX_HISTORY_CAPACITY;

    public void Add(ICommand? command)
    {
        if (command == null)
            return;

        this.commands.AddLast(command);

        if (this.commands.Count > MAX_CAPACITY)
        {
            this.commands.RemoveFirst();
        }
    }

    public ICommand? GetLast()
    {
        if (this.commands.Count == 0)
            return null;

        ICommand lastCommand = this.commands.Last!.Value;
        this.commands.RemoveLast();
        return lastCommand;
    }

    public void Clear()
    {
        this.commands.Clear();
    }
}
