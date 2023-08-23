namespace PrismFeaturesReplacement.ViewModels;

public partial class PageThreeViewModel : BaseViewModel
{
    [ObservableProperty]
    private bool _boolValue;

    public PageThreeViewModel(INavigationService navigationService, IPageDialogService dialogService, IEventAggregator eventAggregator)
        : base(navigationService, dialogService, eventAggregator)
    {
        EventAggregator.SubscribeEvent<BoolParameterEvent>(this, (o, v) => HandleBoolParameterEvent(v.Value));
        EventAggregator.SubscribeEvent<ParameterlessEvent>(this, (_, _) => HandleParameterlessEvent());
    }

    private async void HandleParameterlessEvent()
    {
        await DialogService.DisplayAlertAsync("ParameterlessEvent", "Event received on Page Three", "OK");
    }

    private void HandleBoolParameterEvent(bool value)
    {
        BoolValue = value;
    }

    public override Task Initialize(INavigationParameters parameters)
    {
        return Task.CompletedTask;
    }

    public override void Destroy()
    {
        EventAggregator.UnsubscribeEvent<BoolParameterEvent>(this);
        EventAggregator.UnsubscribeEvent<ParameterlessEvent>(this);
        base.Destroy(); 
    }
}

