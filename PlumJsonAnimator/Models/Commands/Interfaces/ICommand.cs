namespace PlumJsonAnimator.Models.Commands;

public interface ICommand
{
    void Execute();
    void Undo();
}