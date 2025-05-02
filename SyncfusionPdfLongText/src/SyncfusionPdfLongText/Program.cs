using Spectre.Console;
using SyncfusionPdfLongText.Helpers;

try
{
    // The template file is included in the solution.
    const string templateFilePath = @"templates\form.pdf";

    // ***
    // TODO: Change the location of where you want the filled PDF to be written.
    // ***
    var filledPdfFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "filled.pdf");

    PdfFillHelper.FillPdfAndOpen(
        pdfTemplateFilePath: templateFilePath,
        renderedPdfPath: filledPdfFilePath
        );

    //PdfLayouterHelper.ShowIfTextWillFitInTextBoxes(
    //    pdfTemplateFilePath: templateFilePath,
    //    renderedPdfPath: filledPdfFilePath
    //    );

    return 0;
}
catch (Exception ex)
{
    AnsiConsole.MarkupLine("[red]An unhandled exception has occurred:[/]");
    AnsiConsole.WriteException(ex);

    return -1;
}