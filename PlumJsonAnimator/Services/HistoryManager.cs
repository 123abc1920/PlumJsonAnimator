using PlumJsonAnimator.Models.Commands;

namespace PlumJsonAnimator.Services;

public class HistoryManager
{
    private readonly DoneHistory _done = new DoneHistory();
    private readonly UndoneHistory _undone = new UndoneHistory();

    public void DoCommand(ICommand command)
    {
        command.Execute();

        _done.Add(command);
        _undone.Clear();
    }

    public void Undo()
    {
        ICommand? command = _done.GetLast();
        command?.Undo();
        _undone.Add(command);
    }

    public void Redo()
    {
        ICommand? command = _undone.GetLast();
        command?.Execute();
        _done.Add(command);
    }

    public void Clear()
    {
        _undone.Clear();
        _done.Clear();
    }
}
