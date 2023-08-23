namespace PrismFeaturesReplacement.Services;

public class NavigationService : INavigationService
{
    private readonly IServiceProvider services;
    private readonly NavigationPage rootNavigationPage; 
    private readonly SemaphoreSlim navSemaphore = new(1, 1);

    public NavigationService(IServiceProvider services, NavigationPage rootNavigationPage)
    {
        this.services = services;
        this.rootNavigationPage = rootNavigationPage;
    }

    public void Initialize()
    {
        Application.Current.MainPage = rootNavigationPage;
        rootNavigationPage.Popped += NavigationService_Popped;
        rootNavigationPage.PoppedToRoot += NavigationService_PoppedToRoot;
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

    internal NavigationPage GetCurrentNavigationPage(bool isRoot = false)
    {
        var page = Application.Current.MainPage;
        if (page is NavigationPage navPage)
        {
            if (isRoot)
            {
                return navPage;
            }

            if (navPage.CurrentPage is TabbedPage tabbed)
            {
                if (tabbed.CurrentPage is NavigationPage tabbedNavPage)
                {
                    return tabbedNavPage;
                }
            }

            return navPage;
        }

        return null;
    }

    public Task<INavigationResult> NavigateToAsync(string route, INavigationParameters parameters = default)
    {
        return NavigateToAsync(route, parameters);
    }

    public async Task<INavigationResult> NavigateToAsync(string route, INavigationParameters parameters = default, bool replaceRoot = false, bool animated = true)
    {
        try
        {
            await navSemaphore.WaitAsync();
            var (toPage, toViewModel) = ResolvePage(route);
            var currentNavPage = GetCurrentNavigationPage(replaceRoot);

            //Call Initialize on VM, passing in the paramter
            if (toViewModel is not null)
            {
                SetBindingContext(toPage, toViewModel);
                _ = InitializeViewModel(toPage, toViewModel, parameters ?? new NavigationParameters());
            }

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                //Navigate to requested page
                if (replaceRoot)
                {
                    await PushPage(currentNavPage, toPage, animated);
                    RemovePagesTo(currentNavPage, toPage);
                }
                else
                {
                    await PushPage(currentNavPage, toPage, animated);
                }
            });

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
        toPage.BindingContext = toViewModel;
    }

    private async Task InitializeViewModel(Page toPage, BaseViewModel toViewModel, INavigationParameters parameters)
    {
        await Task.Delay(300);

        _ = toViewModel.Initialize(parameters);
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

    private async Task PushPage(NavigationPage navigationPage, Page toPage, bool animated)
    {
        if (toPage is TabbedPage tabbedPage)
        {
            foreach (var childPage in tabbedPage.Children)
            {
                if (childPage is NavigationPage cp)
                {
                    cp.Popped += NavigationService_Popped;
                    cp.PoppedToRoot += NavigationService_PoppedToRoot;
                }
            }
        }
        await navigationPage.Navigation.PushAsync(toPage, animated);
    }

    private void RemovePagesTo(NavigationPage currentNavPage, Page toPage)
    {
        foreach (var page in currentNavPage.Navigation.NavigationStack.ToList())
        {
            if (page != toPage)
            {
                if (page is TabbedPage tabbedPage)
                {
                    DestroyChildrenTabPages(tabbedPage);
                }
                currentNavPage.Navigation.RemovePage(page);
                NavigationService_Popped(currentNavPage, new NavigationEventArgs(page));
            }
        }
    }

    private void DestroyChildrenTabPages(TabbedPage tabbedPage)
    {
        foreach (var childPage in tabbedPage.Children)
        {
            if (childPage is NavigationPage navPage)
            {
                var pageList = navPage.Navigation.NavigationStack.ToList();
                foreach (var page in pageList)
                {
                    NavigationService_Popped(navPage, new NavigationEventArgs(page));
                }
                navPage.Popped -= NavigationService_Popped;
                navPage.PoppedToRoot -= NavigationService_PoppedToRoot;
            }
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

    public Task GoBackToRootAsync(bool isMainRoot = false)
    {
        var navPage = GetCurrentNavigationPage(isMainRoot);
        return navPage.Navigation.PopToRootAsync();
    }

    public Task GoBackAsync(INavigationParameters parameters = default)
    {
        var navPage = GetCurrentNavigationPage();
        if (navPage.Navigation.NavigationStack.Count > 1)
        {
            return navPage.PopAsync();
        }

        var rootNavPage = GetCurrentNavigationPage(true);
        if (rootNavPage.Navigation.NavigationStack.Count > 1)
        {
            return rootNavPage.PopAsync();
        }

        Application.Current.Quit();

        throw new InvalidOperationException("No pages to navigate back to!");
    }
}