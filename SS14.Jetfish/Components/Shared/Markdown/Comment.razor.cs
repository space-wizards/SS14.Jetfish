using Microsoft.AspNetCore.Components;
using MudBlazor;
using SS14.Jetfish.Security.Model;

namespace SS14.Jetfish.Components.Shared.Markdown;

public partial class Comment : MudComponentBase
{
    [Parameter]
    public bool IsEdit { get; set; }

    [Parameter]
    public string Text { get; set; } = string.Empty;

    [Parameter]
    public Guid ProjectId { get; set; }

    [CascadingParameter]
    public Security.Model.User? User { get; set; }

    [Parameter]
    public required DateTime Date { get; set; }

    [Parameter]
    public required User Author { get; set; }

    /// <summary>
    /// Called when the comment is edited and saved
    /// </summary>
    [Parameter]
    public required EventCallback<string> OnEditCallback { get; set; }

    /// <summary>
    /// Called when the comment is deleted.
    /// </summary>
    [Parameter]
    public required EventCallback OnDeleteCallback { get; set; }

    [Inject]
    public IJsApiService JsApiService { get; set; } = null!;

    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    public void ExitCallback(string text)
    {
        IsEdit = false;
    }

    public async Task SaveCallback(string text)
    {
        IsEdit = false;
        await OnEditCallback.InvokeAsync(text);
    }

    private async Task CopyText()
    {
        await JsApiService.CopyToClipboardAsync(Text);
        Snackbar.Add("Copied text to clipboard!", Severity.Success,
            options =>
            {
                options.Icon = Icons.Material.Rounded.Check;
            });
    }
}
