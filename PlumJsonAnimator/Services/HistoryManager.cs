using PlumJsonAnimator.Common.Constants.Command;
using PlumJsonAnimator.Models.Commands;

namespace PlumJsonAnimator.Services;

class HistoryManager
{
    private readonly DoneHistory done = new DoneHistory();
    private readonly UndoneHistory undone = new UndoneHistory();

    public void DoCommand(ICommand command)
    {
        command.Execute();

        done.Add(command);
        undone.Clear();
    }

    public void CtrlZ()
    {
        ICommand? command = done.GetLast();
        undone.Add(command);
    }

    public void CtrlY()
    {
        ICommand? command = undone.GetLast();
        done.Add(command);
    }
}
