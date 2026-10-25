using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models.Resources;
using PlumJsonAnimator.Services;

namespace PlumJsonAnimator.Models.Factories;

public class ResFactory
{
    private readonly GlobalState _globalState;
    private readonly ProjectFilesManager _projectFilesManager;

    public ResFactory(GlobalState globalState, ProjectFilesManager projectFilesManager)
    {
        _globalState = globalState;
        _projectFilesManager = projectFilesManager;
    }

    public Res? CreateRes(ResTypes resType, ResDTO? resDTO = null)
    {
        switch (resType)
        {
            case ResTypes.IMAGE:
                if (!(resDTO is null))
                {
                    return new ImageRes(_projectFilesManager, _globalState, resDTO);
                }
                else
                {
                    return new ImageRes(_projectFilesManager, _globalState);
                }
        }

        return null;
    }
}
