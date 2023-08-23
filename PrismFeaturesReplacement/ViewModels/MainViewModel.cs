namespace PrismFeaturesReplacement.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    [ObservableProperty]
    private bool _isAbsoluteNavigation = true;

    public MainViewModel(INavigationService navigationService, IPageDialogService dialogService, IEventAggregator eventAggregator) : base(navigationService, dialogService, eventAggregator)
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
        return NavigationService.NavigateToAsync(Routes.MainTabbed, new NavigationParameters { { "IsAbsoluteNavigation", IsAbsoluteNavigation } }, IsAbsoluteNavigation);
    }
}