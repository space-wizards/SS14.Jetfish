using Microsoft.AspNetCore.Components;
using MudBlazor;
using Serilog;
using SS14.Jetfish.Core.Types;
using SS14.Jetfish.Helpers;

namespace SS14.Jetfish.Core.Services;

public sealed class UiErrorService
{
    private readonly NavigationManager _navigationManager;
    private readonly IDialogService _dialogService;
    private readonly ISnackbar _snackbar;

    
    public UiErrorService(NavigationManager navigationManager, IDialogService dialogService, ISnackbar snackbar)
    {
        _navigationManager = navigationManager;
        _dialogService = dialogService;
        _snackbar = snackbar;
    }

    public async Task HandleUiError(Exception? exception)
    {
        if (exception is not null and not UiException)
            Log.Error(exception, "An error occured");
        
        if (exception == null || exception.RequiresReload())
        {
            await BlazorUtility.DisplayErrorPopup(_dialogService, _navigationManager, exception);
            return;
        }

        _snackbar.Add(exception.Message, Severity.Error);
    }

}