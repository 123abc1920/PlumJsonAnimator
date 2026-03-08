using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Svg.Skia;
using Microsoft.Extensions.DependencyInjection;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Services;
using PlumJsonAnimator.ViewModels;
using PlumJsonAnimator.Views;

namespace PlumJsonAnimator;

public partial class App : Application
{
    private IServiceProvider serviceProvider;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainViewModelInstance = serviceProvider.GetService<MainWindowViewModel>();
            mainViewModelInstance.initProgram();

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

    public override void RegisterServices()
    {
        base.RegisterServices();

        var services = new ServiceCollection();

        // РЕГИСТРИРУЕМ ВАШ СЕРВИС
        // Вариант 1: Singleton (один экземпляр на всё приложение) - обычно для настроек
        //services.AddSingleton<IAppSettings, AppSettings>();

        // Или если без интерфейса:
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
        services.AddSingleton<Color>();

        // Другие сервисы
        //services.AddSingleton<MainWindowViewModel>();

        serviceProvider = services.BuildServiceProvider();
    }
}

// #ff003b #00ffc4 #011021 #070F19 #000A15 #DCDCDC
