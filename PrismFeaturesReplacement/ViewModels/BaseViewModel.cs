
namespace PrismFeaturesReplacement.ViewModels;

public partial class BaseViewModel : ObservableObject
{ 
    protected IPageDialogService DialogService { get; }
    protected IEventAggregator EventAggregator { get; } 
    protected INavigationService NavigationService { get; }

    [ObservableProperty]
    private string title;

    [ObservableProperty]
    private bool isBusy;  

    public BaseViewModel(INavigationService navigationService, IPageDialogService dialogService, IEventAggregator eventAggregator) 
    { 
        NavigationService = navigationService;
        DialogService = dialogService;
        EventAggregator = eventAggregator; 
    }

    public virtual Task Initialize(INavigationParameters parameters)
    {
        return Task.CompletedTask;
    }

    public virtual void Destroy()
    { 
    }  
}

