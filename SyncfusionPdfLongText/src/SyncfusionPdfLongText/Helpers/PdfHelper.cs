using Humanizer;
using Spectre.Console;
using Syncfusion.Drawing;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.Parsing;

namespace SyncfusionPdfLongText.Helpers;

public static class PdfHelper
{
    private const string LongText1 = "longunbrokencalibertextohmygoshwhyisthissolong";
    private const string LongText2 = "9MM PARABULLUM";

    public static void DemonstrateLongTextIssue(string pdfTemplateFilePath, string renderedPdfPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pdfTemplateFilePath);
        FileHelper.ThrowIfNotExists(pdfTemplateFilePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(renderedPdfPath);

        AnsiConsole.WriteLine($"Loading PDF template file '{pdfTemplateFilePath}' into memory...");
        using var pdfTemplateStream = File.OpenRead(pdfTemplateFilePath);
        using var pdfDocument = new PdfLoadedDocument(pdfTemplateStream);
        AnsiConsole.WriteLine("Done.");
        AnsiConsole.WriteLine("");

        AnsiConsole.WriteLine("Getting Caliber form fields...");
        var caliberTextBoxFields = GetCaliberTextBoxFields(pdfDocument);
        AnsiConsole.WriteLine($"Found {"caliber field".ToQuantity(caliberTextBoxFields.Count, "N0")}.");
        AnsiConsole.WriteLine("Done.");
        AnsiConsole.WriteLine("");

        AnsiConsole.WriteLine("Displaying text size information for all Caliber form fields...");
        foreach (var caliberTextBoxField in caliberTextBoxFields)
        {
            AnsiConsole.MarkupLine("[blue]*** ============================================================================[/]");
            AnsiConsole.MarkupLineInterpolated($"[blue]*** {caliberTextBoxField.Name}:[/]");
            AnsiConsole.MarkupLine("[blue]*** ============================================================================[/]");
            AnsiConsole.WriteLine("");

            CheckStringSize(LongText1, caliberTextBoxField);
            AnsiConsole.WriteLine("");

            CheckStringSize(LongText2, caliberTextBoxField);
            AnsiConsole.WriteLine("");


            AnsiConsole.WriteLine("Done.");
            AnsiConsole.WriteLine("");
        }
        AnsiConsole.WriteLine("Done.");
        AnsiConsole.WriteLine("");
    }


    //
    // Private methods
    //

    private static List<PdfLoadedTextBoxField> GetCaliberTextBoxFields(PdfLoadedDocument pdfDocument)
    {
        List<PdfLoadedTextBoxField> caliberFields = [];
        var enumerator = pdfDocument.Form.Fields.GetEnumerator();

        while (enumerator.MoveNext())
        {
            var pdfField = (PdfField)enumerator.Current;
            if (!pdfField.Name.StartsWith("topmostSubform[0].Page1[0].Q5_CaliberGauge_", StringComparison.Ordinal))
            {
                continue;
            }

            caliberFields.Add((PdfLoadedTextBoxField)pdfField);
        }

        return caliberFields;
    }

    static void CheckStringSize(string text, PdfLoadedTextBoxField textField)
    {
        AnsiConsole.MarkupLineInterpolated($"Checking string [green]'{text}'[/] to see if it will fit within {nameof(PdfLoadedTextBoxField)} [yellow]'{textField.Name}'[/]...");

        Console.WriteLine();
        Console.WriteLine($"TextBox Bounds:");
        Console.WriteLine($"  Width: {textField.Bounds.Width}");
        Console.WriteLine($"  Height: {textField.Bounds.Height}");


        //
        // Mesure the string
        //

        var stringSize = textField.Font.MeasureString(text, new PdfStringFormat(textField.TextAlignment));

        Console.WriteLine();
        Console.WriteLine($"MeaureString:");
        Console.WriteLine($"  Width: {stringSize.Width}");
        Console.WriteLine($"  Height: {stringSize.Height}");


        //
        // The "layouter" determines whether the text will be clipped in the given textbox.
        //

        var layouter = new PdfStringLayouter();
        var layoutResult = layouter.Layout(text, textField.Font, new PdfStringFormat(textField.TextAlignment), new SizeF(textField.Bounds.Width, textField.Bounds.Height));

        Console.WriteLine();
        Console.WriteLine($"Layout result:");
        Console.WriteLine($"  ActualSize:");
        Console.WriteLine($"    Width: {layoutResult.ActualSize.Width}");
        Console.WriteLine($"    Height: {layoutResult.ActualSize.Height}");
        Console.WriteLine($"  LineHeight: {layoutResult.LineHeight}");
        Console.WriteLine($"  Lines:");
        foreach (var line in layoutResult.Lines)
        {
            Console.WriteLine($"    Line:");
            Console.WriteLine($"      LineType: {line.LineType}");
            Console.WriteLine($"      Text: {line.Text}");
            Console.WriteLine($"      Width: {line.Width}");
        }
        Console.WriteLine($"  Remainder: {layoutResult.Remainder}");

        Console.WriteLine();

        if (string.IsNullOrEmpty(layoutResult.Remainder))
        {
            AnsiConsole.MarkupLineInterpolated($"[green]*** Syncfusion.Pdf reports that the string '{text}' WILL fit within the TextBoxField.[/]");
        }
        else
        {
            AnsiConsole.MarkupLineInterpolated($"[red]*** Syncfusion.Pdf reports that the string '{text}' will NOT fit within the TextBoxField.[/]");
        }
    }
}
