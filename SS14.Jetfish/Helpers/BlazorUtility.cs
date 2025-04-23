using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
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
            $"Are you sure you want to delete this {name}? This action cannot be undone!",
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

    /// <summary>
    /// Get the userId from the authentication state task usually provided as a cascading parameter
    /// </summary>
    /// <param name="AuthenticationState">The Authentication state tasl</param>
    /// <returns>The <see cref="Guid"/> of the authenticated user</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the <paramref name="AuthenticationState"/> is null, or if the UserId cannot be determined from the user claims.
    /// </exception>
    /// <remarks>
    /// This method should not be called where it's not certain the user is already authenticated
    /// </remarks>
    public static async Task<Guid> GetUserId(this Task<AuthenticationState>? AuthenticationState)
    {
        if (AuthenticationState == null)
            throw new InvalidOperationException("AuthenticationState is null");

        var auth = await AuthenticationState;
        var userId = auth.User.Claims.GetUserId();
        if (!userId.HasValue)
            throw new InvalidOperationException("UserId is null");

        return userId.Value;
    }
}