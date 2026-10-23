using System.Collections.Generic;
using System.Linq;
using PlumJsonAnimator.Models.SkeletonNameSpace;
using PlumJsonAnimator.Models.SkeletonNameSpace.Attachments;

namespace PlumJsonAnimator.Models.Commands;

public class BoneAnim
{
    public Bone _bone;
    public Animation _animation;
    public BoneAnimation _boneAnimation;

    public BoneAnim(Bone bone, Animation animation, BoneAnimation boneAnimation)
    {
        _bone = bone;
        _animation = animation;
        _boneAnimation = boneAnimation;
    }
}

public class SlotAttach
{
    public Slot _slot;
    public Skin _skin;
    public Attachment _attachment;

    public SlotAttach(Slot slot, Skin skin, Attachment attachment)
    {
        _slot = slot;
        _skin = skin;
        _attachment = attachment;
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
        if (selectedBone == null)
        {
            return;
        }

        _selectedBone = selectedBone;
        _project = project;
        _parent = selectedBone.Parent;

        SnapshotBonesRecursive(_selectedBone);
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
        DeleteBoneReqursion(_selectedBone);
    }

    public void Undo()
    {
        RestoreBoneRecursive(_selectedBone);
        _parent?.Children.Add(_selectedBone);

        foreach (var skinDTO in skins)
        {
            skinDTO._skin.RestoreSlot(skinDTO._slot, skinDTO._attachment);
        }

        foreach (var animDTO in anims)
        {
            animDTO._animation.RestoreBoneAnimation(animDTO._bone, animDTO._boneAnimation);
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
