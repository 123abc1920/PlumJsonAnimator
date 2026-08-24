using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Svg.Skia;
using Microsoft.Extensions.DependencyInjection;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Common.Dialogs;
using PlumJsonAnimator.Models.Common;
using PlumJsonAnimator.Services;
using PlumJsonAnimator.ViewModels;
using PlumJsonAnimator.Views;

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

        services.AddTransient<NewProjectViewModel>();
        services.AddTransient<RenameViewModel>();
        services.AddTransient<ExportPanelGIFViewModel>();
        services.AddTransient<ExportPanelJPGViewModel>();
        services.AddTransient<ExportPanelMP4ViewModel>();
        services.AddTransient<ExportPanelPNGViewModel>();
        services.AddTransient<AppSettingsViewModel>();

        services.AddSingleton<AppSettings>();
        services.AddSingleton<GlobalState>();

        services.AddSingleton<Interpolation>();
        services.AddSingleton<Engine>();

        services.AddSingleton<LocalizationService>();

        services.AddSingleton<TransformModeFactory>();

        services.AddSingleton<Prettify>();
        services.AddSingleton<JsonCode>();
        services.AddSingleton<JsonValidator>();

        services.AddSingleton<ImageExporter>();
        services.AddSingleton<JsonExport>();

        services.AddSingleton<ProjectSettings>();
        services.AddSingleton<ProjectFilesManager>();

        services.AddSingleton<Dialogs>();

        services.AddSingleton<PlumApp>();

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

            var app = _serviceProvider.GetRequiredService<PlumApp>();
            app.Start();

            Application.Current?.Resources.MergedDictionaries.Add(app.Localization.LangResources);

            var mainViewModelInstance = _serviceProvider.GetService<MainWindowViewModel>();
            if (mainViewModelInstance == null)
            {
                throw new InvalidOperationException(
                    "MainWindowViewModel not found in service provider"
                );
            }

            var mainWindow = new MainWindow { DataContext = mainViewModelInstance };
            mainWindow.initViews();
            desktop.MainWindow = mainWindow;
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
