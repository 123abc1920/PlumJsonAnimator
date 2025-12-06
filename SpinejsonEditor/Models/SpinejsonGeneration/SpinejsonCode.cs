using System;
using System.ComponentModel;
using EngineModels;
using Prettify;

namespace SpinejsonGeneration
{
    public class SpinejsonCode : INotifyPropertyChanged
    {
        public String text = "";

        public string Text
        {
            get => text;
            set
            {
                if (text != value)
                {
                    text = value;
                    OnPropertyChanged(nameof(Text));
                }
            }
        }

        public SpinejsonCode() { }

        public void generateCode(Project project)
        {
            text = "{\"skeleton\": {\"spine\": \"4.2.22\"},";

            text += project.mainSkeleton?.generateCode();

            text += ", \"skins\": [], \"animations\": {";

            for (int i = 0; i < project.animations.Count; i++)
            {
                text += project.animations[i].generateCode();
                if (i != project.animations.Count - 1)
                {
                    text += ",";
                }
            }
            text += "}";

            text += "}";

            text = Prettify.Prettify.prettify(text);
            OnPropertyChanged(nameof(Text));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
