namespace PrismFeaturesReplacement.ViewModels;

public partial class MainFlyoutViewModel : BaseViewModel
{  
    public MainFlyoutViewModel(INavigationService navigationService, IPageDialogService dialogService, IEventAggregator eventAggregator)
        : base(navigationService, dialogService, eventAggregator)
    { 
    } 
    public override Task Initialize(INavigationParameters parameters)
    {  
        return base.Initialize(parameters);
    }

    public override void Destroy()
    {
        base.Destroy();
    }

    [RelayCommand]
    private Task NavigateTo(string route)
    {
        return NavigationService.NavigateToAsync(route, navigationMode: Services.NavigationType.ReplaceToRoot);
    }
}

