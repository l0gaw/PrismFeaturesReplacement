namespace PrismFeaturesReplacement.Services;

public class NavigationService : INavigationService
{
    private readonly IServiceProvider services;
    private readonly SemaphoreSlim navSemaphore = new(1, 1);

    public NavigationService(IServiceProvider services)
    {
        this.services = services;
    }

    public void Initialize()
    {
        Application.Current.MainPage = CreateNavigationPage();
    }

    private NavigationPage CreateNavigationPage()
    {
        var rootNavigationPage = services.GetRequiredService<NavigationPage>();
        rootNavigationPage.Popped += NavigationService_Popped;
        rootNavigationPage.PoppedToRoot += NavigationService_PoppedToRoot;
        return rootNavigationPage;
    }

    private void NavigationService_Popped(object sender, NavigationEventArgs e)
    {
        if (e.Page.BindingContext is BaseViewModel viewModel)
        {
            viewModel.Destroy();
        }
    }

    private void NavigationService_PoppedToRoot(object sender, NavigationEventArgs e)
    {
        if (e is PoppedToRootEventArgs ev)
        {
            foreach (var page in ev.PoppedPages)
            {
                if (page.BindingContext is BaseViewModel viewModel)
                {
                    viewModel.Destroy();
                }
            }
        }
    }

    internal NavigationPage GetCurrentNavigationPage() => Application.Current.MainPage switch
    {
        NavigationPage navPage => navPage,
        TabbedPage tabbed when tabbed.CurrentPage is NavigationPage tabbedNavPage => tabbedNavPage,
        FlyoutPage flyout when flyout.Detail is NavigationPage flyoutNavPage => flyoutNavPage,
        _ => null,
    };

    public async Task<INavigationResult> NavigateToAsync(string route, INavigationParameters parameters = default, NavigationMode navigationMode = NavigationMode.Push, bool animated = true)
    {
        try
        {
            await navSemaphore.WaitAsync();
            var (toPage, toViewModel) = ResolvePage(route);
            var currentNavPage = GetCurrentNavigationPage();

            //Call Initialize on VM, passing in the paramter
            if (toViewModel is not null)
            {
                SetBindingContext(toPage, toViewModel);
            }

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await PushPage(currentNavPage, toPage, navigationMode, animated);
            });

            _ = InitializeViewModel(toPage, toViewModel, parameters ?? new NavigationParameters());

            return new NavigationResult();
        }
        catch (Exception ex)
        {
            return new NavigationResult { Exception = ex };
        }
        finally
        {
            navSemaphore.Release();
        }
    }

    private void SetBindingContext(Page toPage, BaseViewModel toViewModel)
    {
        //Set BindingContext
        if (toPage is TabbedPage tabbedPage)
        {
            foreach (var ch in tabbedPage.Children)
            {
                var rootPage = ch is NavigationPage np ? np.CurrentPage : ch;
                var (_, chViewModel) = ResolvePage(rootPage.GetType().Name);
                rootPage.BindingContext = chViewModel;
            }
        }

        if (toPage is FlyoutPage flyoutPage)
        {
            flyoutPage.BindingContext = flyoutPage.Flyout.BindingContext = toViewModel;
            var rootPage = flyoutPage.Detail is NavigationPage np ? np.CurrentPage : flyoutPage.Detail;
            if (rootPage is null)
                return;

            var (_, chViewModel) = ResolvePage(rootPage.GetType().Name);
            rootPage.BindingContext = chViewModel;
        }

        toPage.BindingContext = toViewModel;
    }

    private async Task InitializeViewModel(Page toPage, BaseViewModel toViewModel, INavigationParameters parameters)
    {
        await toViewModel.Initialize(parameters);

        if (toPage is FlyoutPage flyoutPage)
        {
            var rootPage = flyoutPage.Detail is NavigationPage np ? np.CurrentPage : flyoutPage.Detail;
            if (rootPage.BindingContext is BaseViewModel chViewModel)
            {
                _ = chViewModel.Initialize(parameters);
            }
        }

        if (toPage is TabbedPage tabbedPage)
        {
            foreach (var ch in tabbedPage.Children)
            {
                var rootPage = ch is NavigationPage np ? np.CurrentPage : ch;
                if (rootPage.BindingContext is BaseViewModel chViewModel)
                {
                    _ = chViewModel.Initialize(parameters);
                }
            }
        }
    }

    private async Task PushPage(NavigationPage navigationPage, Page toPage, NavigationMode navigationMode, bool animated)
    {
        if (toPage is TabbedPage tabbedPage)
        {
            PushTabbedPage(tabbedPage);
            return;
        }

        if (toPage is FlyoutPage flyoutPage)
        {
            PushFlyoutPage(flyoutPage);
            return;
        }

        if (Application.Current.MainPage is FlyoutPage rootFlyoutPage)
        {
            rootFlyoutPage.IsPresented = false;
        }

        if (navigationMode == NavigationMode.AbsoluteNavigation &&
           (Application.Current.MainPage is TabbedPage || Application.Current.MainPage is FlyoutPage))
        {
            var previousMainPage = Application.Current.MainPage;
            var rootNavigationPage = CreateNavigationPage();
            await rootNavigationPage.Navigation.PushAsync(toPage, animated);
            Application.Current.MainPage = rootNavigationPage;
            _ = DestroyPreviousMainPage(previousMainPage, toPage);
            return;
        }

        if (navigationMode == NavigationMode.AbsoluteNavigation ||
            navigationMode == NavigationMode.ReplaceToRoot)
        {
            if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                var hasBackButton = NavigationPage.GetHasBackButton(toPage);
                NavigationPage.SetHasBackButton(toPage, false);
                await navigationPage.Navigation.PushAsync(toPage, animated);
                DestroyPreviousPagesTo(navigationPage, toPage);
                NavigationPage.SetHasBackButton(toPage, hasBackButton);
            }
            else
            {
                navigationPage.Navigation.InsertPageBefore(toPage, navigationPage.Navigation.NavigationStack[0]);
                await navigationPage.Navigation.PopToRootAsync();
            }

            return;
        }

        await navigationPage.Navigation.PushAsync(toPage, animated);
    }

    private void PushFlyoutPage(FlyoutPage flyoutPage)
    {
        var previousMainPage = Application.Current.MainPage;
        if (flyoutPage.Detail is NavigationPage np)
        {
            np.Popped += NavigationService_Popped;
            np.PoppedToRoot += NavigationService_PoppedToRoot;
        }
        Application.Current.MainPage = flyoutPage;
        DestroyPreviousMainPage(previousMainPage);
    }

    private void PushTabbedPage(TabbedPage tabbedPage)
    {
        var previousMainPage = Application.Current.MainPage;
        foreach (var childPage in tabbedPage.Children)
        {
            if (childPage is NavigationPage np)
            {
                np.Popped += NavigationService_Popped;
                np.PoppedToRoot += NavigationService_PoppedToRoot;
            }
        }
        Application.Current.MainPage = tabbedPage;
        DestroyPreviousMainPage(previousMainPage);
    }

    private Task DestroyPreviousMainPage(Page previousMainPage, Page newPage = null)
    {
        switch (previousMainPage)
        {
            case TabbedPage tabbedPage:
                return DestroyPreviousTabbedPage(tabbedPage);
            case FlyoutPage flyoutPage:
                return DestroyPreviousFlyoutPage(flyoutPage);
            case NavigationPage navigationPage:
                DestroyPreviousPagesTo(navigationPage, newPage);
                break;
        }
        return Task.CompletedTask;
    }

    private async Task DestroyPreviousTabbedPage(TabbedPage tabbedPage)
    {
        await DestroyChildrenTabPages(tabbedPage);
        NavigationService_Popped(tabbedPage, new NavigationEventArgs(tabbedPage));
    }

    private async Task DestroyPreviousFlyoutPage(FlyoutPage flyoutPage)
    {
        await DestroyDetailPage(flyoutPage);
        NavigationService_Popped(flyoutPage, new NavigationEventArgs(flyoutPage));
    }

    private void DestroyPreviousPagesTo(NavigationPage currentNavPage, Page toPage)
    {
        var navigationStack = currentNavPage.Navigation.NavigationStack.ToList();
        foreach (var page in navigationStack)
        {
            if (page == toPage)
            {
                continue;
            }

            if (navigationStack.Count == 1)
            {
                currentNavPage.PoppedToRoot -= NavigationService_PoppedToRoot;
                currentNavPage.Popped -= NavigationService_Popped;
                NavigationService_Popped(currentNavPage, new NavigationEventArgs(page));
            }
            else
            {
                currentNavPage.Navigation.RemovePage(page);
            }
        }
    }

    private async Task DestroyDetailPage(FlyoutPage flyoutPage)
    {
        if (flyoutPage.Detail is NavigationPage navPage)
        {
            await navPage.PopToRootAsync();
            NavigationService_Popped(navPage, new NavigationEventArgs(navPage.CurrentPage));
            navPage.Popped -= NavigationService_Popped;
            navPage.PoppedToRoot -= NavigationService_PoppedToRoot;
            return;
        }

        NavigationService_Popped(flyoutPage, new NavigationEventArgs(flyoutPage.Detail));
    }

    private async Task DestroyChildrenTabPages(TabbedPage tabbedPage)
    {
        foreach (var childPage in tabbedPage.Children)
        {
            if (childPage is NavigationPage navPage)
            {
                await navPage.PopToRootAsync();
                NavigationService_Popped(navPage, new NavigationEventArgs(navPage.CurrentPage));
                navPage.Popped -= NavigationService_Popped;
                navPage.PoppedToRoot -= NavigationService_PoppedToRoot;
                continue;
            }

            NavigationService_Popped(tabbedPage, new NavigationEventArgs(childPage));
        }
    }

    private (Page, BaseViewModel) ResolvePage(string route)
    {
        if (RoutingExtensions.VmToPageBindings.TryGetValue(route, out (Type, Type) pageVmTypes))
        {
            var page = (Page)services.GetRequiredService(pageVmTypes.Item1);
            var vm = (BaseViewModel)services.GetRequiredService(pageVmTypes.Item2);
            return (page, vm);
        }

        throw new InvalidOperationException($"Route {route} not registered!");
    }

    public Task GoBackToRootAsync()
    {
        var navPage = GetCurrentNavigationPage();
        return navPage.Navigation.PopToRootAsync();
    }

    public Task GoBackAsync(INavigationParameters parameters = default)
    {
        var navPage = GetCurrentNavigationPage();
        if (navPage.Navigation.NavigationStack.Count > 1)
        {
            return navPage.PopAsync();
        }

        Application.Current.Quit();

        throw new InvalidOperationException("No pages to navigate back to!");
    }
}