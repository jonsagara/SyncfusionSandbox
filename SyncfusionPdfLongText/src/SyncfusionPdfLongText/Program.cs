using Spectre.Console;
using Syncfusion.Licensing;
using SyncfusionPdfLongText.Helpers;

try
{
    

    PdfHelper.DemonstrateLongTextIssue(
        pdfTemplateFilePath: @"C:\Users\Jon\Desktop\form4473.202308.en.pdf",
        renderedPdfPath: @"C:\Users\Jon\Desktop\rendered.pdf"
        );
    return 0;
}
catch (Exception ex)
{
    AnsiConsole.MarkupLine("[red]An unhandled exception has occurred:[/]");
    AnsiConsole.WriteException(ex);
    return -1;
}