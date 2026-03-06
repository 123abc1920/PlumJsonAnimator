using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Svg.Skia;
using Common.Constants;
using PlumJsonAnimator.ViewModels;
using PlumJsonAnimator.Views;

namespace PlumJsonAnimator;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainViewModelInstance = new MainWindowViewModel();
            mainViewModelInstance.initProgram();

            var mainWindow = new MainWindow { DataContext = mainViewModelInstance };
            mainWindow.initViews();
            desktop.MainWindow = mainWindow;

            ConstantsClass.viewModel = mainViewModelInstance;
        }

        GC.KeepAlive(typeof(SvgImageExtension).Assembly);
        GC.KeepAlive(typeof(Avalonia.Svg.Skia.Svg).Assembly);

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove = BindingPlugins
            .DataValidators.OfType<DataAnnotationsValidationPlugin>()
            .ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}

// #ff003b #00ffc4 #011021 #070F19 #000A15 #DCDCDC
