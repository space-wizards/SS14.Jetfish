using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace SS14.Jetfish.Components.Shared.Markdown;

public partial class MarkdownEditor : MudComponentBase
{
    private string Text { set; get; } = string.Empty;

    [Parameter]
    public string ParameterText { get; set; } = string.Empty;

    [Parameter]
    public EventCallback<string> OnSubmitClick { get; set; }

    [Parameter]
    public string SubmitText { get; set; } = "Submit";

    [Parameter]
    public EventCallback<string> OnCancelClick { get; set; }
    /// <summary>
    /// If set to a value, shows a cancel button.
    /// </summary>
    [Parameter]
    public string? CancelText { get; set; }

    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    [Parameter]
    public bool SpinnerOnSubmit { get; set; } = false;

    public MarkdownEditorInterop Editor { get; set; } = null!;

    private bool _isLoading = false;

    private readonly Guid _editorId = Guid.NewGuid();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        if (!string.IsNullOrEmpty(ParameterText))
            Text = ParameterText;
        else
            Text = string.Empty;

        await Editor.SetText(Text);
    }

    private async Task FetchText()
    {
        var text = await Editor.GetText();
        Text = text ?? string.Empty;
    }

    private async Task OnPreviewInteraction(TabInteractionEventArgs arg)
    {
        if (arg.PanelIndex == 1)
        {
            // Switching to preview.
            await FetchText();
            await JsRuntime.InvokeVoidAsync("cmInterop.destroy", _editorId);
        }
    }

    private async Task ButtonClicked(EventCallback<string> callback, bool showSpinner)
    {
        if (showSpinner && SpinnerOnSubmit)
        {
            _isLoading = true;
            StateHasChanged();
        }

        await FetchText();
        await callback.InvokeAsync(Text);
    }

    /// <summary>
    /// Resets the editor and sets the text to <see cref="ParameterText"/>
    /// </summary>
    public async Task Reset()
    {
        if (!string.IsNullOrEmpty(ParameterText))
            Text = ParameterText;
        else
            Text = string.Empty;

        await JsRuntime.InvokeVoidAsync("cmInterop.setValue", _editorId, Text);
        _isLoading = false;
    }
}
