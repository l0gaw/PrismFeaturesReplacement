namespace PrismFeaturesReplacement.Services;

public interface INavigationService
{
    void Initialize();
    Task<INavigationResult> NavigateToAsync(string route, INavigationParameters parameters = default, bool replaceRoot = false, bool animated = true);
    Task GoBackToRootAsync(bool isMainRoot = false);
    Task GoBackAsync(INavigationParameters parameters = default);
}
