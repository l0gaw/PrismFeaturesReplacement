using Mopups.Services;

namespace PrismFeaturesReplacement;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .RegisterDependencies()
            .RegisterRoutes()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}

    private static MauiAppBuilder RegisterDependencies(this MauiAppBuilder builder)
    {
        var serviceCollection = builder.Services;  
        serviceCollection.AddSingleton((_) => MopupService.Instance);
        serviceCollection.AddSingleton<INavigationService, NavigationService>();
        serviceCollection.AddSingleton<IEventAggregator, EventAggregator>();
        serviceCollection.AddSingleton<IPageDialogService, PageDialogService>();

#if ANDROID || IOS
        PlatformRegistrations.RegisterTypes(serviceCollection);
#endif
        return builder;
    }

    private static MauiAppBuilder RegisterRoutes(this MauiAppBuilder builder)
    {
        //Put your custom navigation bar here
        //builder.Services.AddTransient<NavigationPage>((_) => new NavigationBarPage());
        builder.Services.AddTransient<NavigationPage>(); 
        builder.Services.RegisterRoute<MainPage, MainViewModel>();
        builder.Services.RegisterRoute<MainTabbedPage, MainTabbedViewModel>();
        builder.Services.RegisterRoute<MainFlyoutPage, MainFlyoutViewModel>();
        builder.Services.RegisterRoute<PageOne, PageOneViewModel>();
        builder.Services.RegisterRoute<PageTwo, PageTwoViewModel>();
        builder.Services.RegisterRoute<PageThree, PageThreeViewModel>();
        builder.Services.RegisterRoute<PageFour, PageFourViewModel>();
        return builder;
    }

}



