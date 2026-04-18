namespace PlumJsonAnimator.Models.Interfaces
{
    /// <summary>
    /// Interface for al objects which can be renamed from UI
    /// </summary>
    public interface IRenamable
    {
        string GetName { get; set; }

        public void SetName(string? name);
    }
}
