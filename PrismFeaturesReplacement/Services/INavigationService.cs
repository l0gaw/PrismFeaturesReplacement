namespace PrismFeaturesReplacement.Services;

public interface INavigationService
{
    void Initialize();
    Task<INavigationResult> NavigateToAsync(string route, INavigationParameters parameters = default, NavigationType navigationMode = NavigationType.Push, bool animated = true);
    Task GoBackToRootAsync();
    Task GoBackAsync(INavigationParameters parameters = default);
}

public enum NavigationType
{
    Push,
    ReplaceToRoot,
    AbsoluteNavigation,
    ModalNavigation
}