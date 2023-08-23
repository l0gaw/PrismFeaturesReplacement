namespace PrismFeaturesReplacement.Services;

public class PageDialogService : IPageDialogService
{
    public Task<bool> DisplayAlertAsync(string title, string message, string acceptButton, string cancelButton)
    {
        return Application.Current.MainPage.DisplayAlert(title, message, acceptButton, cancelButton);
    }

    public Task DisplayAlertAsync(string title, string message, string cancelButton)
    {
        return Application.Current.MainPage.DisplayAlert(title, message, cancelButton);
    }
}