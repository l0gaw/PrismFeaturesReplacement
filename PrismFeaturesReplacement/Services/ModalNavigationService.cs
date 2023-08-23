namespace PrismFeaturesReplacement.Services;

public class ModalNavigationService : IModalNavigationService
{
    private readonly IServiceProvider services;
    private readonly IPopupNavigation popupNavigationService;

    public ModalNavigationService(IServiceProvider services, IPopupNavigation popupNavigationService)
    {
        this.services = services;
        this.popupNavigationService = popupNavigationService;
        this.popupNavigationService.Popped += Mopup_Popped;
    }

    private void Mopup_Popped(object sender, Mopups.Events.PopupNavigationEventArgs e)
    {
        if (e.Page?.BindingContext is BaseViewModel baseViewModel)
        {
            baseViewModel?.Destroy();
        }
    }

    public async Task<INavigationResult> NavigateToAsync(string route, INavigationParameters parameters = default, bool animated = true)
    {
        try
        {
            var (toModal, toViewModel) = ResolveModal(route);
            toModal.BindingContext = toViewModel;
            _ = InitializeViewModel(toModal, toViewModel, parameters ?? new NavigationParameters());

            await popupNavigationService.PushAsync(toModal, animated);

            return new NavigationResult();
        }
        catch (Exception ex)
        {
            return new NavigationResult { Exception = ex };
        }
    }

    private async Task InitializeViewModel(Page toPage, BaseViewModel toViewModel, INavigationParameters parameters)
    {
        await Task.Delay(300);

        _ = toViewModel.Initialize(parameters);
    }

    private (PopupPage, BaseViewModel) ResolveModal(string route)
    {
        if (RoutingExtensions.VmToModalBindings.TryGetValue(route, out (Type, Type) pageVmTypes))
        {
            var page = (PopupPage)services.GetRequiredService(pageVmTypes.Item1);
            var vm = (BaseViewModel)services.GetRequiredService(pageVmTypes.Item2);
            return (page, vm);
        }

        throw new InvalidOperationException($"Route {route} not registered!");
    }

    public Task GoBackAsync()
    {
        return popupNavigationService.PopAsync();
    }
}

