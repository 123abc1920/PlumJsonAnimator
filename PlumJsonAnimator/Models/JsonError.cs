using PlumJsonAnimator.Models.Interfaces;
using PlumJsonAnimator.Services;

// TODO: should fix json error logic
namespace PlumJsonAnimator.Models
{
    /// <summary>
    /// Json error objects
    /// </summary>
    public class JsonError : INotifyable
    {
        private LocalizationService _localizationService;
        private string _errorText = "";

        public string ErrorText
        {
            get => _errorText;
            set
            {
                _errorText = value;
                if (value == this._localizationService.GetMessage(LocalizationConsts.JSON_VALID))
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
            this._localizationService = localizationService;
        }
    }
}
