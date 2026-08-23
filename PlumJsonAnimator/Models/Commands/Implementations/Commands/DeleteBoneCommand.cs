using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PlumJsonAnimator.Models.SkeletonNameSpace;
using Tmds.DBus.Protocol;

namespace PlumJsonAnimator.Models.Commands;

class DeleteBoneCommand : ICommand
{
    private class BoneAnim
    {
        public Bone bone;
        public Animation animation;
        public BoneAnimation boneAnimation;
    }

    private class SlotAttach
    {
        public Slot slot;
        public Skin skin;
        public Attachment attachment;
    }

    private readonly Bone _selectedBone;
    private readonly Project? _project;
    private readonly Bone? _parent;

    private List<SlotAttach> skins = new List<SlotAttach>();
    private List<BoneAnim> anims = new List<BoneAnim>();

    private readonly List<Bone> _savedBonesBranch = new List<Bone>();

    public DeleteBoneCommand(Bone selectedBone, Project? project)
    {
        this._selectedBone = selectedBone;
        this._project = project;
        this._parent = selectedBone.Parent;

        SnapshotBonesRecursive(this._selectedBone);
    }

    private void SnapshotBonesRecursive(Bone? bone)
    {
        if (bone == null)
            return;
        _savedBonesBranch.Add(bone);
        foreach (Bone child in bone.Children)
        {
            SnapshotBonesRecursive(child);
        }
    }

    public void Execute()
    {
        DeleteBoneReqursion(this._selectedBone);
    }

    public void Undo()
    {
        RestoreBoneRecursive(this._selectedBone);
        this._parent?.Children.Add(this._selectedBone);
    }

    private void DeleteBoneReqursion(Bone? bone)
    {
        if (bone != null && bone.Parent != null)
        {
            foreach (Slot s in bone.Slots)
            {
                _project?.DeleteSlotFromProject(s);
            }
            foreach (Bone b in bone.Children.ToList())
            {
                DeleteBoneReqursion(b);
            }
            _project?.DeleteBoneFromProject(bone);
        }
    }

    private void RestoreBoneRecursive(Bone? bone)
    {
        if (bone == null)
            return;

        _project?.RestoreBone(bone);

        var originalChildren = _savedBonesBranch.Where(b => b.Parent == bone).ToList();
        foreach (Bone b in originalChildren)
        {
            RestoreBoneRecursive(b);
            bone.AddChildren(b);
            b.Parent = bone;
        }
    }
}
