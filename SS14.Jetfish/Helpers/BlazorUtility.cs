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

    public static async Task DisplayModifiedPopup(IDialogService dialogService, NavigationManager navigationManager)
    {
        await dialogService.ShowMessageBox(
            "Error",
            "The resource has been modified by someone else. Please reload the page to restore state.",
            yesText: "Reload Page"
        );

        navigationManager.Refresh(true);
    }
}