using NavigationType = PrismFeaturesReplacement.Services.NavigationType;

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
        base.Destroy();
    }

    [RelayCommand]
    private Task NavigateToTabbed()
    {
        return NavigationService.NavigateToAsync(Routes.MainTabbed, new NavigationParameters { { "IsAbsoluteNavigation", IsAbsoluteNavigation } }, NavigationType.Push);
    }

    [RelayCommand]
    private Task NavigateToFlyout()
    {
        return NavigationService.NavigateToAsync(Routes.MainFlyout, new NavigationParameters { { "IsAbsoluteNavigation", IsAbsoluteNavigation } }, NavigationType.Push);
    }

    [RelayCommand]
    private Task NavigateToPage()
    {
        return NavigationService.NavigateToAsync(Routes.Page1, new NavigationParameters { { "IsAbsoluteNavigation", IsAbsoluteNavigation } }, IsAbsoluteNavigation ? NavigationType.AbsoluteNavigation : NavigationType.Push);
    }
}