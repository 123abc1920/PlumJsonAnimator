using System.Collections.Generic;
using PlumJsonAnimator.Common.Constants;

namespace PlumJsonAnimator.Models.Commands;

abstract class IHistory
{
    protected LinkedList<ICommand> _commands = new LinkedList<ICommand>();
    private readonly int MAX_CAPACITY = Consts.MAX_HISTORY_CAPACITY;

    public void Add(ICommand? command)
    {
        if (command == null)
            return;

        _commands.AddLast(command);

        if (_commands.Count > MAX_CAPACITY)
        {
            _commands.RemoveFirst();
        }
    }

    public ICommand? GetLast()
    {
        if (_commands.Count == 0)
            return null;

        ICommand lastCommand = _commands.Last!.Value;
        _commands.RemoveLast();
        return lastCommand;
    }

    public void Clear()
    {
        _commands.Clear();
    }
}
