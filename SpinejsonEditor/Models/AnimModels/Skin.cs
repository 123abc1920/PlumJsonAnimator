using System.Collections.Generic;
using System.Net.Mail;
using Avalonia.Controls;
using Newtonsoft.Json;
using Tmds.DBus.Protocol;

namespace AnimModels
{
    public class Skin
    {
        public string Name { get; set; } = "defualt";
        public Dictionary<Bone, Slot> BoneSlotBinding = new Dictionary<Bone, Slot>();

        public Skin() { }

        public Skin(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Binds a bone with a slot
        /// </summary>
        /// <param name="b"></param>
        /// <param name="s"></param>
        public void BindBoneAndSlot(Bone b, Slot s)
        {
            if (BoneSlotBinding.ContainsKey(b))
            {
                BoneSlotBinding[b] = s;
            }
            else
            {
                BoneSlotBinding.Add(b, s);
            }
            b.Slot = s;
            s.BoundedBone = b;
        }

        /// <summary>
        /// Unbinds a bone and a slot
        /// </summary>
        /// <param name="b"></param>
        public void UnbindBoneAndSlot(Bone b)
        {
            if (b == null)
            {
                return;
            }

            if (b.Slot != null)
            {
                b.Slot.BoundedBone = null;
            }
            b.Slot = null;
            BoneSlotBinding.Remove(b);
        }

        /// <summary>
        /// Returns bone slot in this skin
        /// </summary>
        /// <param name="bone"></param>
        /// <returns></returns>
        public Slot? GetSlot(Bone bone)
        {
            if (BoneSlotBinding.ContainsKey(bone))
            {
                return BoneSlotBinding[bone];
            }
            return null;
        }

        /// <summary>
        /// Must be called before skin deleting. Deletes bone slots
        /// </summary>
        public void DeleteSkin()
        {
            foreach (Bone b in BoneSlotBinding.Keys)
            {
                if (b.Slot != null)
                {
                    b.Slot.BoundedBone = null;
                }
                b.Slot = null;
            }
        }

        /// <summary>
        /// Draws skin
        /// </summary>
        /// <param name="canvas"></param>
        public void DrawSkin(Canvas canvas)
        {
            foreach (Slot s in BoneSlotBinding.Values)
            {
                s.drawSlot(canvas);
            }
        }

        public SkinData generateJSONData()
        {
            Dictionary<string, Dictionary<string, AttachmentData>> attachments =
                new Dictionary<string, Dictionary<string, AttachmentData>>();

            foreach (Slot s in BoneSlotBinding.Values)
            {
                attachments[s.Name] = new Dictionary<string, AttachmentData>();
                attachments[s.Name][s.Attachment] = new AttachmentData
                {
                    Name = s.Attachment,
                    Width = 100,
                    Height = 100,
                };
            }

            return new SkinData { Name = this.Name, Attachments = attachments };
        }

        public string generateCode()
        {
            return JsonConvert.SerializeObject(
                generateJSONData(),
                Constants.ConstantsClass.jsonSettings
            );
        }
    }
}

public class AttachmentData
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("width")]
    public int Width { get; set; }

    [JsonProperty("height")]
    public int Height { get; set; }
}

public class SkinData
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("attachments")]
    public Dictionary<string, Dictionary<string, AttachmentData>> Attachments { get; set; }
}
