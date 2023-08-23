namespace PrismFeaturesReplacement.Models;

//A little modified copy of NavigationResult from the Prism Library(https://github.com/PrismLibrary/Prism)
//They've been used for compatibility reasons  
public interface INavigationResult
{
    bool Success => Exception is null;

    Exception Exception { get; }
}

public record NavigationResult : INavigationResult
{
    public bool Success => Exception is null;

    public Exception Exception { get; init; }
}
