using PlumJsonAnimator.Models.SkeletonNameSpace;

namespace PlumJsonAnimator.Models.Commands;

class AddBoneCommand : ICommand
{
    private readonly Bone _selectedBone;
    private readonly Project _project;

    private Bone? _newBone;

    public AddBoneCommand(Bone selectedBone, Project project)
    {
        this._selectedBone = selectedBone;
        this._project = project;
    }

    public void Execute()
    {
        if (this._newBone != null)
        {
            _project?.AddBoneToProject(this._newBone, this._selectedBone);
            return;
        }

        if (_selectedBone != null && _selectedBone.IsBone)
        {
            this._newBone = _project?.MainSkeleton?.AddBoneToParent(_selectedBone.id);
        }
    }

    public void Undo()
    {
        _project.DeleteBoneFromProject(this._newBone);
    }
}
