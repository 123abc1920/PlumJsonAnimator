using PlumJsonAnimator.Models.SkeletonNameSpace;

namespace PlumJsonAnimator.Models.Commands;

class AddBoneCommand : ICommand
{
    private readonly Bone _selectedBone;
    private readonly Project _project;

    private Bone? _newBone;

    public AddBoneCommand(Bone selectedBone, Project project)
    {
        _selectedBone = selectedBone;
        _project = project;
    }

    public void Execute()
    {
        if (_newBone != null)
        {
            _project?.AddBoneToProject(_newBone, _selectedBone);
            return;
        }

        if (_selectedBone != null && _selectedBone.IsBone)
        {
            _newBone = _project?.MainSkeleton?.AddBoneToParent(_selectedBone._id);
        }
    }

    public void Undo()
    {
        _project.DeleteBoneFromProject(_newBone);
    }
}
