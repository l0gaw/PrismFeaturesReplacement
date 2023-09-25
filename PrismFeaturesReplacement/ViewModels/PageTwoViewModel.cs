namespace PrismFeaturesReplacement.ViewModels;

public partial class PageTwoViewModel : BaseViewModel
{
    [ObservableProperty]
    private bool _boolValue;

    public PageTwoViewModel(INavigationService navigationService, IPageDialogService dialogService, IEventAggregator eventAggregator) : base(navigationService, dialogService, eventAggregator)
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
    private void PublishParameterlessEvent() {
        EventAggregator.PublishEvent<ParameterlessEvent>(new ());
    }

    [RelayCommand]
    private void PublishBoolParameterEvent()
    {
        EventAggregator.PublishEvent<BoolParameterEvent>(new(BoolValue));
    }
}

