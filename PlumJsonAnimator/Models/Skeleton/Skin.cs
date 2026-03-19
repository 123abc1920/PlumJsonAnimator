using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Newtonsoft.Json;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models.Interfaces;
using PlumJsonAnimator.Models.Resources;

namespace PlumJsonAnimator.Models.SkeletonNameSpace
{
    public class Skin : INotifyable
    {
        private string _name = "default";

        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }
        public Dictionary<Slot, Attachment?> SlotAttachmentBinding =
            new Dictionary<Slot, Attachment?>();

        private GlobalState globalState;

        public Skin(GlobalState globalState)
        {
            this.globalState = globalState;
        }

        public Skin(string name, GlobalState globalState)
        {
            this.Name = $"{name}{Counter.GenerateName()}";
            this.globalState = globalState;
        }

        public void BindSlotAttachment(Slot s, Attachment a)
        {
            if (s == null)
            {
                return;
            }

            if (SlotAttachmentBinding.ContainsKey(s))
            {
                SlotAttachmentBinding[s] = a;
            }
            else
            {
                SlotAttachmentBinding.Add(s, a);
            }
            s.UpdateAttachment();
        }

        /// <summary>
        /// Adds slots to skin without binding
        /// </summary>
        /// <param name="s"></param>
        /// <param name="a"></param>
        public void AddSlot(Slot s)
        {
            if (SlotAttachmentBinding.ContainsKey(s))
            {
                SlotAttachmentBinding[s] = null;
            }
            else
            {
                SlotAttachmentBinding.Add(s, null);
            }
        }

        public void DeleteSlot(Slot s)
        {
            if (SlotAttachmentBinding.ContainsKey(s))
            {
                SlotAttachmentBinding.Remove(s);
            }
        }

        /// <summary>
        /// Draws skin
        /// </summary>
        /// <param name="canvas"></param>
        public void DrawSkin(Canvas canvas)
        {
            foreach (Slot s in SlotAttachmentBinding.Keys.OrderBy(slot => slot.CurrentDrawOrderOffset))
            {
                s.drawSlot(canvas);
            }
        }

        public bool ContainsSlot(Slot s)
        {
            return SlotAttachmentBinding.ContainsKey(s);
        }

        public bool ContainsRes(Res res)
        {
            foreach (Attachment? a in SlotAttachmentBinding.Values)
            {
                if (a?.GetRes() == res)
                {
                    return true;
                }
            }
            return false;
        }

        public bool ContainsAndRemoveRes(Res res)
        {
            foreach (Slot s in SlotAttachmentBinding.Keys)
            {
                if (SlotAttachmentBinding[s] != null && SlotAttachmentBinding[s].GetRes() == res)
                {
                    SlotAttachmentBinding[s] = null;
                    s.UpdateAttachment();
                    return true;
                }
            }
            return false;
        }

        public string GetImagePath(Slot s)
        {
            if (SlotAttachmentBinding.ContainsKey(s) && SlotAttachmentBinding[s] != null)
            {
                return ((ImageAttachment)SlotAttachmentBinding[s]).getPath();
            }
            else
            {
                return "Пусто";
            }
        }

        /// <summary>
        /// Returns all slots bounded with this bone
        /// </summary>
        /// <param name="b"></param>
        /// <returns>A list of slots</returns>
        public List<Slot> GetSlots(Bone b)
        {
            List<Slot> slots = new List<Slot>();
            foreach (Slot s in SlotAttachmentBinding.Keys)
            {
                if (s.BoundedBone == b)
                {
                    slots.Add(s);
                }
            }
            return slots;
        }

        /// <summary>
        /// Checks whether the slot can be drawn
        /// </summary>
        /// <param name="s"></param>
        /// <returns>Returns true, if slot can be drawn</returns>
        public bool isSlotDrawable(Slot s)
        {
            if (SlotAttachmentBinding.ContainsKey(s) && SlotAttachmentBinding[s] != null)
            {
                return true;
            }
            return false;
        }

        public Attachment? GetAttachment(Slot s)
        {
            if (SlotAttachmentBinding.ContainsKey(s))
            {
                return SlotAttachmentBinding[s];
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
            return JsonConvert.SerializeObject(generateJSONData(), this.globalState.jsonSettings);
        }
    }

    public class SkinData
    {
        [JsonProperty("name")]
        public required string Name { get; set; }

        [JsonProperty("attachments")]
        public required Dictionary<
            string,
            Dictionary<string, AttachmentData>
        > Attachments { get; set; }
    }
}
