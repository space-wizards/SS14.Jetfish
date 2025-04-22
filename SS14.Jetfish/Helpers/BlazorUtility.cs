using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace SS14.Jetfish.Helpers;

public static class BlazorUtility
{
    public static IEnumerable<string> MaxCharacters(string ch, int max)
    {
        if (!string.IsNullOrEmpty(ch) && max < ch.Length)
            yield return $"Max {max} characters";
    }

    public static async Task<bool> ConfirmDelete(IDialogService dialogService, string name)
    {
        var result = await dialogService.ShowMessageBox(
            "Warning ඞ",
            $"Are you sure you want to delete this {name}?\nThis action cannot be undone!",
            yesText:"Delete!", cancelText:"Cancel");

        if (!result.HasValue || !result.Value)
            return false;

        return true;
    }

    public static async Task DisplayErrorPopup(IDialogService dialogService, NavigationManager navigationManager, Exception? exception = null)
    {
        var message = exception?.Message ?? "An error occured.";
        
        await dialogService.ShowMessageBox(
            "Error",
            $"{message} Please reload the page to restore state.",
            yesText: "Reload Page"
        );

        navigationManager.Refresh(true);
    }
}