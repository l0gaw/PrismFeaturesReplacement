namespace PrismFeaturesReplacement.Services;

public interface INavigationService
{
    void Initialize();
    Task<INavigationResult> NavigateToAsync(string route, INavigationParameters parameters = default, NavigationMode navigationMode = NavigationMode.Push, bool animated = true);
    Task GoBackToRootAsync();
    Task GoBackAsync(INavigationParameters parameters = default);
}

public enum NavigationMode
{
    Push,
    ReplaceToRoot,
    AbsoluteNavigation,
    ModalNavigation
}