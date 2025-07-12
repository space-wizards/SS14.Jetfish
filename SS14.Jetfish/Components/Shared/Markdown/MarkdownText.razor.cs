using Markdig;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace SS14.Jetfish.Components.Shared.Markdown;

public partial class MarkdownText : ComponentBase
{
    [Parameter]
    public required string Text { get; set; }

    public static readonly MarkdownPipeline MarkdownPipeline = new MarkdownPipelineBuilder()
        .UseEmphasisExtras() // Allows for ~# and stuff
        .UsePreciseSourceLocation()
        .UseMathematics()
        .UseFooters()
        .UseFootnotes()
        .UseMediaLinks()
        .Build();

    public static readonly MudMarkdownStyling Styling = new MudMarkdownStyling()
    {
        CodeBlock = { Theme = CodeBlockTheme.UnikittyDarkBase16 },
    };
}
