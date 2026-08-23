using System.Collections.Generic;

namespace PlumJsonAnimator.Models.Commands;

class DoneHistory : IHistory
{
    public IReadOnlyCollection<ICommand> GetAll()
    {
        return this.commands;
    }
}
