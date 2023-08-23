namespace PrismFeaturesReplacement.Extensions;

public static class RoutingExtensions
{
    public static Dictionary<string, (Type, Type)> VmToPageBindings { get; } = new();
    public static Dictionary<string, (Type, Type)> VmToModalBindings { get; } = new();

    public static void RegisterRoute<VType, VmType>(this IServiceCollection serviceCollection, string route = default, bool isSingleton = false)
    where VType : Page
    where VmType : BaseViewModel
    {
        var pageType = typeof(VType);
        var vmType = typeof(VmType);
        if (string.IsNullOrWhiteSpace(route))
        {
            route = pageType.Name;
        }

        VmToPageBindings.Add(route, new(pageType, vmType));
        if (isSingleton)
        {
            serviceCollection.AddSingleton(typeof(VType));
            serviceCollection.AddSingleton(typeof(VmType));
        }
        else
        {
            serviceCollection.AddTransient(typeof(VType));
            serviceCollection.AddTransient(typeof(VmType));
        }
    }

    public static void RegisterModalRoute<VType, VmType>(this IServiceCollection serviceCollection, string route = default)
    where VType : PopupPage
    where VmType : BaseViewModel
    {
        var popupType = typeof(VType);
        var vmType = typeof(VmType);
        if (string.IsNullOrWhiteSpace(route))
        {
            route = popupType.Name;
        }

        VmToModalBindings.Add(route, new(popupType, vmType));
        serviceCollection.AddTransient(typeof(VType));
        serviceCollection.AddTransient(typeof(VmType));
    }
}

