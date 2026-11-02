using System;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Common.Dialogs;
using PlumJsonAnimator.Models.Common;
using PlumJsonAnimator.Services;
using PlumJsonAnimator.ViewModels;

namespace PlumJsonAnimator.Models.Factories;

public class ViewModelFactory
{
    private GlobalState _globalState;
    private Dialogs _dialogs;
    private AppSettings _appSettings;
    private LocalizationService _localizationService;
    private ImageExporter _imageExporter;
    private PlumApp _plumApp;

    public ViewModelBase? GetViewModel(DialogType viewType)
    {
        if (viewType == DialogType.SETTINGS)
        {
            return new AppSettingsViewModel(
                _globalState,
                _dialogs,
                _appSettings,
                _localizationService,
                _imageExporter,
                _plumApp
            );
        }
        if (viewType == DialogType.NEWPROJECT)
        {
            return new NewProjectViewModel(
                _globalState,
                _dialogs,
                _appSettings,
                _localizationService,
                _imageExporter,
                _plumApp
            );
        }
        if (viewType == DialogType.RENAME)
        {
            return new RenameViewModel(
                _globalState,
                _dialogs,
                _appSettings,
                _localizationService,
                _imageExporter,
                _plumApp
            );
        }
        if (viewType == DialogType.EXPORT_JPG)
        {
            return new ExportPanelJPGViewModel(
                _globalState,
                _dialogs,
                _imageExporter,
                _appSettings,
                _localizationService,
                _plumApp
            );
        }
        if (viewType == DialogType.EXPORT_PNG)
        {
            return new ExportPanelPNGViewModel(
                _globalState,
                _dialogs,
                _imageExporter,
                _appSettings,
                _localizationService,
                _plumApp
            );
        }
        if (viewType == DialogType.EXPORT_GIF)
        {
            return new ExportPanelGIFViewModel(
                _globalState,
                _dialogs,
                _imageExporter,
                _appSettings,
                _localizationService,
                _plumApp
            );
        }
        if (viewType == DialogType.EXPORT_MP4)
        {
            return new ExportPanelMP4ViewModel(
                _globalState,
                _dialogs,
                _imageExporter,
                _appSettings,
                _localizationService,
                _plumApp
            );
        }

        return null;
    }

    public ViewModelFactory(
        GlobalState globalState,
        Dialogs dialogs,
        AppSettings appSettings,
        LocalizationService localizationService,
        ImageExporter imageExporter,
        PlumApp plumApp
    )
    {
        _globalState = globalState;
        _dialogs = dialogs;
        _appSettings = appSettings;
        _localizationService = localizationService;
        _imageExporter = imageExporter;
        _plumApp = plumApp;
    }
}
