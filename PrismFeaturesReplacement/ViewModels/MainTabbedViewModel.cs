namespace PrismFeaturesReplacement.ViewModels;

public partial class MainTabbedViewModel : BaseViewModel
{
    private const int PageThreeIndex = 2;

    [ObservableProperty]
    private bool _isNavigationBarVisible;

    [ObservableProperty]
    private int _selectedTabIndex;

    public MainTabbedViewModel(INavigationService navigationService, IPageDialogService dialogService, IEventAggregator eventAggregator)
        : base(navigationService, dialogService, eventAggregator)
    {
        EventAggregator.SubscribeEvent<BoolParameterEvent>(this, (_, _) => HandleBoolParameterEvent());
    }

    private void HandleBoolParameterEvent()
    {
        SelectedTabIndex = PageThreeIndex;
    }

    public override Task Initialize(INavigationParameters parameters)
    {
        if (parameters.TryGetValue("IsAbsoluteNavigation", out bool value))
        {
            IsNavigationBarVisible = !value;
        }

        return base.Initialize(parameters);
    }

    public override void Destroy()
    {
        EventAggregator.UnsubscribeEvent<BoolParameterEvent>(this);
        base.Destroy();
    } 
}

