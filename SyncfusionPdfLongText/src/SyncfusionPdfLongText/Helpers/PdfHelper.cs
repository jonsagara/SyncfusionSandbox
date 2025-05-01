using Spectre.Console;
using Syncfusion.Drawing;
using Syncfusion.Pdf.Graphics;
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
        using var pdfTemplateStream = GetTemplateAsMemoryStream(pdfTemplateFilePath);
        using var pdfDocument = new PdfLoadedDocument(file: pdfTemplateStream);
        AnsiConsole.WriteLine("Done.");
        AnsiConsole.WriteLine("");

        AnsiConsole.WriteLine("Getting Caliber form fields...");
        GetCaliberTextBoxFields(pdfDocument);
        //AnsiConsole.WriteLine($"Found {"caliber field".ToQuantity(caliberTextBoxFields.Count, "N0")}.");
        AnsiConsole.WriteLine("Done.");
        AnsiConsole.WriteLine("");

        //AnsiConsole.WriteLine("Displaying text size information for all Caliber form fields...");
        //foreach (var caliberTextBoxField in caliberTextBoxFields)
        //{
        //    AnsiConsole.MarkupLine("[blue]*** ============================================================================[/]");
        //    AnsiConsole.MarkupLineInterpolated($"[blue]*** {caliberTextBoxField.Name}:[/]");
        //    AnsiConsole.MarkupLine("[blue]*** ============================================================================[/]");
        //    AnsiConsole.WriteLine("");

        //    CheckStringSize(LongText1, caliberTextBoxField);
        //    AnsiConsole.WriteLine("");

        //    CheckStringSize(LongText2, caliberTextBoxField);
        //    AnsiConsole.WriteLine("");


        //    AnsiConsole.WriteLine("Done.");
        //    AnsiConsole.WriteLine("");
        //}
        //AnsiConsole.WriteLine("Done.");
        //AnsiConsole.WriteLine("");
    }


    //
    // Private methods
    //

    private static MemoryStream GetTemplateAsMemoryStream(string pdfTemplateFilePath)
    {
        var pdfTemplateMemoryStream = new MemoryStream();

        using (var pdfTemplateFileStream = File.OpenRead(pdfTemplateFilePath))
        {
            pdfTemplateFileStream.CopyTo(pdfTemplateMemoryStream);

            pdfTemplateMemoryStream.Position = 0L;
        }

        return pdfTemplateMemoryStream;
    }

    private static List<PdfLoadedTextBoxField> GetCaliberTextBoxFields(PdfLoadedDocument pdfDocument)
    {
        List<PdfLoadedTextBoxField> caliberFields = [];

        if (pdfDocument.Form.Fields.TryGetField($"topmostSubform[0].Page1[0].Q5_CaliberGauge_1", out PdfLoadedField pdfField1))
        {
            var caliberField = (PdfLoadedTextBoxField)pdfField1;

            CheckStringSize(LongText1, caliberField);
            AnsiConsole.WriteLine("");

            //caliberFields.Add((PdfLoadedTextBoxField)pdfField1);
        }

        if (pdfDocument.Form.Fields.TryGetField($"topmostSubform[0].Page1[0].Q5_CaliberGauge_2", out PdfLoadedField pdfField2))
        {
            var caliberField = (PdfLoadedTextBoxField)pdfField2;

            CheckStringSize(LongText2, caliberField);
            AnsiConsole.WriteLine("");

            //caliberFields.Add((PdfLoadedTextBoxField)pdfField2);
        }

        return caliberFields;
    }

    static void CheckStringSize(string text, PdfLoadedTextBoxField textBoxField)
    {
        AnsiConsole.MarkupLineInterpolated($"Checking string [green]'{text}'[/] to see if it will fit within {nameof(PdfLoadedTextBoxField)} [yellow]'{textBoxField.Name}'[/]...");

        Console.WriteLine();
        Console.WriteLine($"TextBox Bounds:");
        Console.WriteLine($"  Width: {textBoxField.Bounds.Width}");
        Console.WriteLine($"  Height: {textBoxField.Bounds.Height}");


        // Calling MeasureString alters the state of the PdfLoadedTextBoxField, causing PdfStringLayouter to return
        //   0 for Actual Width and Height.
        ////
        //// Mesure the string
        ////

        //var stringSize = textBoxField.Font.MeasureString(text, new PdfStringFormat(textBoxField.TextAlignment));

        //Console.WriteLine();
        //Console.WriteLine($"MeaureString:");
        //Console.WriteLine($"  Width: {stringSize.Width}");
        //Console.WriteLine($"  Height: {stringSize.Height}");


        //
        // The "layouter" determines whether the text will be clipped in the given textbox.
        //

        // We can't just measure the raw text. We have to fill the textbox first, and then measure the text
        //   in the context of the textbox.
        textBoxField.Text = text;

        var layouter = new PdfStringLayouter();
        var layoutResult = layouter.Layout(text, textBoxField.Font, new PdfStringFormat(textBoxField.TextAlignment), new SizeF(textBoxField.Bounds.Width, textBoxField.Bounds.Height));

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
