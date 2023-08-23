namespace PrismFeaturesReplacement.Services;

public interface IModalNavigationService
{
    Task<INavigationResult> NavigateToAsync(string route, INavigationParameters parameters = default, bool animated = true);
    Task GoBackAsync();
}