using Spectre.Console;
using SyncfusionPdfLongText.Helpers;

try
{
    var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

    var templateFilePath = Path.Combine(desktopPath, "template.pdf");
    var filledPdfFilePath = Path.Combine(desktopPath, "filled.pdf");

    PdfLayouterHelper.ShowIfTextWillFitInTextBoxes(
        pdfTemplateFilePath: templateFilePath,
        renderedPdfPath: filledPdfFilePath
        );

    PdfFillHelper.FillPdfAndOpen(
        pdfTemplateFilePath: templateFilePath,
        renderedPdfPath: filledPdfFilePath
        );

    return 0;
}
catch (Exception ex)
{
    AnsiConsole.MarkupLine("[red]An unhandled exception has occurred:[/]");
    AnsiConsole.WriteException(ex);

    return -1;
}