using PlumJsonAnimator.Common.Constants.Command;
using PlumJsonAnimator.Models.Commands;

namespace PlumJsonAnimator.Services;

public class HistoryManager
{
    private readonly DoneHistory done = new DoneHistory();
    private readonly UndoneHistory undone = new UndoneHistory();

    public void DoCommand(ICommand command)
    {
        command.Execute();

        done.Add(command);
        undone.Clear();
    }

    public void Undo()
    {
        ICommand? command = done.GetLast();
        command?.Undo();
        undone.Add(command);
    }

    public void Redo()
    {
        ICommand? command = undone.GetLast();
        command?.Execute();
        done.Add(command);
    }

    public void Clear()
    {
        this.undone.Clear();
        this.done.Clear();
    }
}
