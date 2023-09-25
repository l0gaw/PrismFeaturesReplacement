namespace PrismFeaturesReplacement.ViewModels;

public partial class PageFourViewModel : BaseViewModel
{   
    public PageFourViewModel(INavigationService navigationService, IPageDialogService dialogService, IEventAggregator eventAggregator)
        : base(navigationService, dialogService, eventAggregator)
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
    private Task GoBack()
    {
        return NavigationService.GoBackAsync();
    }
}

