using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;

namespace SS14.Jetfish.Components.Shared.Markdown;

public partial class MarkdownEditor : MudComponentBase
{
    private const string MarkdownPlaceholder = "*Go* **write** a __masterpiece__!";
    private string Text { set; get; } = string.Empty;

    [Parameter]
    public EventCallback<MouseEventArgs> OnSubmitClick { get; set; }
    [Parameter]
    public string SubmitText { get; set; } = "Submit";

    [Parameter]
    public EventCallback<MouseEventArgs> OnCancelClick { get; set; }
    /// <summary>
    /// If set to a value, shows a cancel button.
    /// </summary>
    [Parameter]
    public string? CancelText { get; set; }

    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        await JsRuntime.InvokeVoidAsync("cmInterop.initialize", "editor-container");
        await JsRuntime.InvokeVoidAsync("cmInterop.setValue", MarkdownPlaceholder);
    }

    private async Task FetchText()
    {
        var text = await JsRuntime.InvokeAsync<string?>("cmInterop.getValue");
        Text = text ?? string.Empty;
    }

    private async Task OnPreviewInteraction(TabInteractionEventArgs arg)
    {
        await FetchText();
    }

    private async Task ButtonClicked(EventCallback<MouseEventArgs> callback)
    {
        await FetchText();
        await callback.InvokeAsync();
    }
}
