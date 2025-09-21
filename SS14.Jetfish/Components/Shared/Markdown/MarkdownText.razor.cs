using Markdig;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace SS14.Jetfish.Components.Shared.Markdown;

public partial class MarkdownText : ComponentBase
{
    [Parameter]
    public string? Class { get; set; }

    [Parameter]
    public required string? Text { get; set; }

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

    public static readonly MudMarkdownStyling Styling = new()
    {
        CodeBlock = { Theme = CodeBlockTheme.UnikittyDarkBase16 },
    };

    private static readonly MudMarkdownProps Props = new()
    {
        Heading =
        {
            OverrideTypo = OverrideTypo,
        },
    };

    private static Typo OverrideTypo(Typo x)
    {
        return x switch
        {
            Typo.h1 => Typo.h4,
            Typo.h2 => Typo.h5,
            Typo.h3 => Typo.h6,
            Typo.h4 or Typo.h5 or Typo.h6 => Typo.h6,
            _ => x,
        };
    }
}
