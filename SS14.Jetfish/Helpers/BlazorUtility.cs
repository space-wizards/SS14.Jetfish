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
            $"Are you sure you want to delete this {name}?\nDeleting can not be undone!",
            yesText:"Delete!", cancelText:"Cancel");

        if (!result.HasValue || !result.Value)
            return false;

        return true;
    }
}