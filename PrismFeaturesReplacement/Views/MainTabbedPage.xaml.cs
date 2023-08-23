namespace PrismFeaturesReplacement.Views;

public partial class MainTabbedPage : TabbedPage
{
    public static readonly BindableProperty SelectedTabPageIndexProperty = BindableProperty.Create(
    nameof(SelectedTabPageIndex),
    typeof(int),
    typeof(MainTabbedPage),
    default,
    propertyChanged: OnSelectedTabPageIndexPropertyChanged);

    public int SelectedTabPageIndex
    {
        get => (int)GetValue(SelectedTabPageIndexProperty);
        set => SetValue(SelectedTabPageIndexProperty, value);
    }

    private static void OnSelectedTabPageIndexPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var mainPage = bindable as MainTabbedPage;
        if (newValue is null)
            return;

        var index = (int)newValue;
        mainPage.CurrentPage = mainPage.Children[index];
    }

    protected override void OnCurrentPageChanged()
    {
        base.OnCurrentPageChanged();
        SelectedTabPageIndex = Children.IndexOf(CurrentPage);
    }

    public MainTabbedPage()
    {
        InitializeComponent();
    }
}


