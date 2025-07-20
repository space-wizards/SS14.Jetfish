using Markdig;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace SS14.Jetfish.Components.Shared.Markdown;

public partial class MarkdownText : ComponentBase
{
    [Parameter]
    public required string Text { get; set; }

    // TODO: Not use mudmarkdown, use markdig instead, its less broken. (For example, emphasis extras is not working)
    public static readonly MarkdownPipeline MarkdownPipeline = new MarkdownPipelineBuilder()
        .UseEmphasisExtras()
        .UsePreciseSourceLocation()
        .UseMathematics()
        .UseFooters()
        .UseFootnotes()
        .UseMediaLinks()
        .UseDiagrams()
        .UseGenericAttributes()
        .Build();

    public static readonly MudMarkdownStyling Styling = new MudMarkdownStyling()
    {
        CodeBlock = { Theme = CodeBlockTheme.UnikittyDarkBase16 },
    };
}
