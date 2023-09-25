
namespace PrismFeaturesReplacement.Views;

public partial class FlyoutMenuPage : ContentPage
{
    public static readonly BindableProperty SelectMenuCommandProperty = BindableProperty.Create(
    nameof(SelectMenuCommand),
    typeof(ICommand),
    typeof(FlyoutMenuPage),
    default);

    //public static readonly BindableProperty SelectMenuCommandParameterProperty = BindableProperty.Create(
    //nameof(SelectMenuCommand),
    //typeof(object),
    //typeof(FlyoutMenuPage),
    //default);

    public ICommand SelectMenuCommand
    {
        get => (ICommand)GetValue(SelectMenuCommandProperty);
        set => SetValue(SelectMenuCommandProperty, value);
    }

    //public object SelectMenuCommandParameter
    //{
    //    get => GetValue(SelectMenuCommandParameterProperty);
    //    set => SetValue(SelectMenuCommandParameterProperty, value);
    //}

    public FlyoutMenuPage()
    {
        InitializeComponent();
    }
}

public class FlyoutPageItem 
{ 
    public string Title { get; set; }
    public string Route { get; set; }
}
