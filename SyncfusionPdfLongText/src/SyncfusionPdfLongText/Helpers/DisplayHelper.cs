using Spectre.Console;

namespace SyncfusionPdfLongText.Helpers;

public static class DisplayHelper
{
    public static void Header(string text)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);

        AnsiConsole.MarkupLineInterpolated($"[blue]*** ============================================================================[/]");
        AnsiConsole.MarkupLineInterpolated($"[blue]    {text}[/]");
        AnsiConsole.MarkupLineInterpolated($"[blue]*** ============================================================================[/]");
    }
}
