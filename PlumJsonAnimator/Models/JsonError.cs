using PlumJsonAnimator.Models.Interfaces;
using PlumJsonAnimator.Services;

// TODO: should fix json error logic
namespace PlumJsonAnimator.Models
{
    public class JsonError : INotifyable
    {
        private LocalizationService localizationService;

        private string _errorText = "";

        public string ErrorText
        {
            get => _errorText;
            set
            {
                _errorText = value;
                if (value == this.localizationService.GetMessage(LocalizationConsts.JSON_VALID))
                {
                    this.isOk = true;
                }
                else
                {
                    this.isOk = false;
                }
                OnPropertyChanged(nameof(ErrorText));
            }
        }

        public bool isOk = true;

        public JsonError(LocalizationService localizationService)
        {
            this.localizationService = localizationService;
        }
    }
}
