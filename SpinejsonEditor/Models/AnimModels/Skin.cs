using System;
using System.Collections.Generic;
using AnimModels;
using Avalonia.Controls;
using Newtonsoft.Json;

namespace AnimModels
{
    public class Skin
    {
        public string Name { get; set; } = "defualt";
        public Dictionary<Slot, Attachment> SlotAttachmentBinding =
            new Dictionary<Slot, Attachment>();

        public Skin() { }

        public Skin(string name)
        {
            this.Name = name;
        }

        public void BindSlotAttachment(Slot s, Attachment a)
        {
            if (SlotAttachmentBinding.ContainsKey(s))
            {
                SlotAttachmentBinding[s] = a;
            }
            else
            {
                SlotAttachmentBinding.Add(s, a);
            }
        }

        /// <summary>
        /// Draws skin
        /// </summary>
        /// <param name="canvas"></param>
        public void DrawSkin(Canvas canvas)
        {
            foreach (Slot s in SlotAttachmentBinding.Keys)
            {
                s.drawSlot(canvas);
            }
        }

        public bool ContainsSlot(Slot s)
        {
            return SlotAttachmentBinding.ContainsKey(s);
        }

        public string GetImagePath(Slot s)
        {
            return ((ImageAttachment)SlotAttachmentBinding[s]).getPath();
        }

        public Slot? GetSlot(Bone b)
        {
            foreach (Slot s in SlotAttachmentBinding.Keys)
            {
                if (s.BoundedBone == b)
                {
                    return s;
                }
            }
            return null;
        }

        public SkinData generateJSONData()
        {
            Dictionary<string, Dictionary<string, AttachmentData>> attachments =
                new Dictionary<string, Dictionary<string, AttachmentData>>();

            foreach (Slot s in SlotAttachmentBinding.Keys)
            {
                attachments[s.Name] = new Dictionary<string, AttachmentData>();
                attachments[s.Name][SlotAttachmentBinding[s].Name] = SlotAttachmentBinding[s]
                    .generateJSONData();
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

public class SkinData
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("attachments")]
    public Dictionary<string, Dictionary<string, AttachmentData>> Attachments { get; set; }
}
