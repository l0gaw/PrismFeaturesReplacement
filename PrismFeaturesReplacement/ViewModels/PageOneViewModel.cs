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
    }

    [RelayCommand]
    private Task Navigate()
    {
        return NavigationService.NavigateToAsync(Routes.Page4);
    } 
}

