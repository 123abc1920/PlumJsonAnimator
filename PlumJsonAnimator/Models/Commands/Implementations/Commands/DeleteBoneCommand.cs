using System.Collections.Generic;
using System.Linq;
using Avalonia.Metadata;
using PlumJsonAnimator.Models.SkeletonNameSpace;

namespace PlumJsonAnimator.Models.Commands;

public class BoneAnim
{
    public Bone bone;
    public Animation animation;
    public BoneAnimation boneAnimation;

    public BoneAnim(Bone bone, Animation animation, BoneAnimation boneAnimation)
    {
        this.bone = bone;
        this.animation = animation;
        this.boneAnimation = boneAnimation;
    }
}

public class SlotAttach
{
    public Slot slot;
    public Skin skin;
    public Attachment attachment;

    public SlotAttach(Slot slot, Skin skin, Attachment attachment)
    {
        this.slot = slot;
        this.skin = skin;
        this.attachment = attachment;
    }
}

class DeleteBoneCommand : ICommand
{
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

        foreach (var skinDTO in skins)
        {
            skinDTO.skin.RestoreSlot(skinDTO.slot, skinDTO.attachment);
        }

        foreach (var animDTO in anims)
        {
            animDTO.animation.RestoreBoneAnimation(animDTO.bone, animDTO.boneAnimation);
        }
    }

    private void DeleteBoneReqursion(Bone? bone)
    {
        if (bone != null && bone.Parent != null)
        {
            foreach (Slot s in bone.Slots)
            {
                var newSkins = _project?.DeleteSlotFromProject(s);
                foreach (var e in newSkins)
                {
                    skins.Add(e);
                }
            }
            foreach (Bone b in bone.Children.ToList())
            {
                DeleteBoneReqursion(b);
            }
            var newAnims = _project?.DeleteBoneFromProject(bone);
            foreach (var e in newAnims)
            {
                anims.Add(e);
            }
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
