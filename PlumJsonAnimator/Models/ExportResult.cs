namespace PlumJsonAnimator.Models
{
    /// <summary>
    /// All possible results of export
    /// </summary>
    public enum ExportResult
    {
        SUCCESS = 0,
        NO_FOLDER,
        INCORRECT_JSON,
        NO_FFMPEG,
        FFMPEG_ERROR,
        PROJECT_IS_NULL,
    }
}
