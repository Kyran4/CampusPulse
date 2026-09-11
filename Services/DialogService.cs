namespace CampusPulse.Services;

public class DialogService
{
    public Task ShowAlert(string title, string message)
    {
        return Application.Current.MainPage.DisplayAlert(title, message, "OK");
    }

    public Task<bool> ShowConfirm(string title, string message)
    {
        return Application.Current.MainPage.DisplayAlert(title, message, "Yes", "No");
    }
}
