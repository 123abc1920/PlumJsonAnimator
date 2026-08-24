using PlumJsonAnimator.Models.Interfaces;
using PlumJsonAnimator.Services;
using ReactiveUI;

// TODO: should fix json error logic
namespace PlumJsonAnimator.Models
{
    /// <summary>
    /// Json error objects
    /// </summary>
    public class JsonError : ReactiveObject
    {
        private readonly LocalizationService _localizationService;
        private string _errorText = "";
        private bool _isOk = true;

        public string ErrorText
        {
            get => _errorText;
            set
            {
                this.RaiseAndSetIfChanged(ref _errorText, value);

                var validMessage = _localizationService.GetMessage(LocalizationConsts.JSON_VALID);
                IsOk = (value == validMessage);
            }
        }

        public bool IsOk
        {
            get => _isOk;
            set => this.RaiseAndSetIfChanged(ref _isOk, value);
        }

        public JsonError(LocalizationService localizationService)
        {
            _localizationService = localizationService;
            _errorText = _localizationService.GetMessage(LocalizationConsts.JSON_VALID);
        }
    }
}
