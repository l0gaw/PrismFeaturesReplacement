namespace PrismFeaturesReplacement;

public partial class App : Application
{
    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        ServiceProvider = serviceProvider;
    }

    public IServiceProvider ServiceProvider { get; }

    protected override Window CreateWindow(IActivationState activationState)
    {
        _ = Initialize();
        return base.CreateWindow(activationState);
    }

    //Initialize the first navigation here
    private async Task Initialize()
    {
        try
        { 
            var navigationService = ServiceProvider.GetRequiredService<INavigationService>(); 
            navigationService.Initialize();
            var result = await navigationService.NavigateToAsync(Routes.Main);
            if (!result.Success)
            {
                Debug.WriteLine(result.Exception);
                Debugger.Break();
            } 
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    } 
}

