using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace SS14.Jetfish.Components.Shared.Markdown;

public partial class MarkdownEditor : MudComponentBase, IAsyncDisposable
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

    /// <summary>
    /// If you are allowed to submit empty text.
    /// </summary>
    [Parameter]
    public bool AllowEmpty { get; set; }

    /// <summary>
    /// The maximum length allowed in this input. Set to 0 to disable.
    /// </summary>
    /// <remarks>
    /// Too high of values will cause SignalR to have a stroke as the text exceeds the maximum packet size.
    /// </remarks>
    [Parameter]
    public int MaxLength { get; set; } = 0;

    public string Error { get; set; } = string.Empty;

    public MarkdownEditorInterop Editor { get; set; } = null!;

    private bool _isLoading = false;

    protected override void OnParametersSet()
    {
        if (!string.IsNullOrEmpty(ParameterText))
            Text = ParameterText;
        else
            Text = string.Empty;
    }

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
            await JsRuntime.InvokeVoidAsync("cmInterop.destroy", Editor.EditorGuid);
        }
    }

    private async Task ButtonClicked(EventCallback<string> callback, bool showSpinner, bool checkError)
    {
        if (showSpinner && SpinnerOnSubmit)
        {
            _isLoading = true;
            StateHasChanged();
        }

        await FetchText();

        if (checkError)
        {
            Error = string.Empty;

            if (MaxLength != 0 && Text.Length > MaxLength)
            {
                Error = $"Text exceeds maximum length {MaxLength}";
            }

            if (Text.Length == 0)
            {
                Error = "Cannot submit empty text";
            }

            if (!string.IsNullOrEmpty(Error))
            {
                _isLoading = false;
                return;
            }
        }

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

        await JsRuntime.InvokeVoidAsync("cmInterop.setValue", Editor.EditorGuid, Text);
        _isLoading = false;
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            await JsRuntime.InvokeVoidAsync("cmInterop.destroy", Editor.EditorGuid);
        }
        catch (JSDisconnectedException)
        {
            // we can safely ignore that.
        }
    }
}
