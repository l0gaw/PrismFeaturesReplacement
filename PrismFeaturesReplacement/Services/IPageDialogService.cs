namespace PrismFeaturesReplacement.Services;

public interface IPageDialogService
{
    Task<bool> DisplayAlertAsync(string title, string message, string acceptButton, string cancelButton);
    Task DisplayAlertAsync(string title, string message, string cancelButton);
}
