using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Svg.Skia;
using Microsoft.Extensions.DependencyInjection;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Services;
using PlumJsonAnimator.ViewModels;
using PlumJsonAnimator.Views;
using SukiUI;
using SukiUI.Models;

namespace PlumJsonAnimator;

public partial class App : Application
{
    private IServiceProvider? _serviceProvider;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void RegisterServices()
    {
        base.RegisterServices();

        var services = new ServiceCollection();

        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<AppSettings>();
        services.AddSingleton<ProjectSettings>();
        services.AddSingleton<ProjectManager>();
        services.AddSingleton<Interpolation>();
        services.AddSingleton<Prettify>();
        services.AddSingleton<JsonCode>();
        services.AddSingleton<JsonValidator>();
        services.AddSingleton<ImageExporter>();
        services.AddSingleton<JsonExport>();
        services.AddSingleton<GlobalState>();
        services.AddSingleton<TransformModeFactory>();
        services.AddSingleton<Engine>();
        services.AddSingleton<LocalizationService>();

        _serviceProvider = services.BuildServiceProvider();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var theme = SukiUI.SukiTheme.GetInstance();
        var myColor = Avalonia.Media.Color.Parse("#ff003b");

        theme.ChangeColorTheme(new SukiUI.Models.SukiColorTheme("PlumAccent", myColor, myColor));

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            if (_serviceProvider == null)
            {
                throw new InvalidOperationException(
                    "Services not registered. Call RegisterServices first."
                );
            }

            var mainViewModelInstance = _serviceProvider.GetService<MainWindowViewModel>();

            if (mainViewModelInstance == null)
            {
                throw new InvalidOperationException(
                    "MainWindowViewModel not found in service provider"
                );
            }

            mainViewModelInstance.initProgram();

            var mainWindow = new MainWindow { DataContext = mainViewModelInstance };

            mainWindow.initViews();
            desktop.MainWindow = mainWindow;

            var localization = _serviceProvider.GetRequiredService<LocalizationService>();
            Application.Current?.Resources.MergedDictionaries.Add(localization.LangResources);
        }

        GC.KeepAlive(typeof(SvgImageExtension).Assembly);
        GC.KeepAlive(typeof(Avalonia.Svg.Skia.Svg).Assembly);

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        var dataValidationPluginsToRemove = BindingPlugins
            .DataValidators.OfType<DataAnnotationsValidationPlugin>()
            .ToArray();

        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}
// #ff003b #00ffc4 #011021 #070F19 #000A15 #DCDCDC
