namespace PrismFeaturesReplacement.ViewModels;

public partial class PageOneViewModel : BaseViewModel
{   
    public PageOneViewModel(INavigationService navigationService, IPageDialogService dialogService, IEventAggregator eventAggregator) : base(navigationService, dialogService, eventAggregator)
    {  
    }

    public override Task Initialize(INavigationParameters parameters)
    {
        return Task.CompletedTask;
    }

    public override void Destroy()
    {
        base.Destroy();
    }

    [RelayCommand]
    private Task Navigate()
    {
        return NavigationService.NavigateToAsync(Routes.Page4);
    }

    [RelayCommand]
    private Task NavigateToMainPage()
    {
        return NavigationService.NavigateToAsync(Routes.Main, navigationMode: Services.NavigationMode.AbsoluteNavigation);
    }
}

