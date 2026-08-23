namespace PlumJsonAnimator.Models.Commands;

interface ICommand
{
    void Execute();
    void Undo();
}