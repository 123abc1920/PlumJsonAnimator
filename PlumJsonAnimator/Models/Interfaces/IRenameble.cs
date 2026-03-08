namespace PlumJsonAnimator.Models.Interfaces
{
    public interface IRenamable
    {
        string GetName { get; set; }

        public abstract void SetName(string? name);
    }
}
