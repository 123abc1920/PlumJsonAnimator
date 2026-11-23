using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Services;

namespace PlumJsonAnimator.Models.Resources;

/// <summary>
/// Image resources, which are uploaded by user
/// </summary>
public class ImageRes : Res
{
    public ImageRes(ProjectFilesManager projectManager, GlobalState globalState)
        : base(projectManager, globalState) { }

    public ImageRes(
        ProjectFilesManager projectManager,
        GlobalState globalState,
        string path,
        string name,
        string ext
    )
        : base(projectManager, globalState)
    {
        Name = name;
        Ext = ext;
        Path = path;
    }

    public ImageRes(ProjectFilesManager projectManager, GlobalState globalState, ResDTO resDTO)
        : base(projectManager, globalState)
    {
        Name = resDTO.ImageName!;
        Ext = resDTO.ImageExt!;
        Path = resDTO.ImagePath!;
    }
}
