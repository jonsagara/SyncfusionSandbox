using Spectre.Console;
using Syncfusion.Drawing;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Parsing;

namespace SyncfusionPdfLongText.Helpers;

public static class PdfLayouterHelper
{
    public static void ShowIfTextWillFitInTextBoxes(string pdfTemplateFilePath, string renderedPdfPath)
    {
        //
        // Validate the arguments.
        //

        ArgumentException.ThrowIfNullOrWhiteSpace(pdfTemplateFilePath);
        FileHelper.ThrowIfNotExists(pdfTemplateFilePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(renderedPdfPath);


        //
        // Display a header in the console.
        //

        DisplayHelper.Header("Show TextBox Layouter Results");
        AnsiConsole.WriteLine();


        //
        // Load the template file into a PdfLoadedDocument instance.
        //

        AnsiConsole.WriteLine($"Loading PDF template file '{pdfTemplateFilePath}' into memory...");
        using var pdfTemplateStream = TemplateHelper.GetTemplateAsMemoryStream(pdfTemplateFilePath);
        using var pdfDocument = new PdfLoadedDocument(file: pdfTemplateStream);
        AnsiConsole.WriteLine("Done.");
        AnsiConsole.WriteLine();


        //
        // Describe the text box bounds of the two Caliber text boxes on the PDF form.
        //

        DescribeCaliberTextBoxFields(pdfDocument);
        AnsiConsole.WriteLine();
    }


    //
    // Private methods
    //

    private static void DescribeCaliberTextBoxFields(PdfLoadedDocument pdfDocument)
    {
        if (pdfDocument.Form.Fields.TryGetField(Constants.CaliberField1Name, out PdfLoadedField pdfField1))
        {
            var caliberField = (PdfLoadedTextBoxField)pdfField1;

            DisplayStringSize(Constants.LongCaliber1, caliberField);
            AnsiConsole.WriteLine("");
        }

        if (pdfDocument.Form.Fields.TryGetField(Constants.CaliberField2Name, out PdfLoadedField pdfField2))
        {
            var caliberField = (PdfLoadedTextBoxField)pdfField2;

            DisplayStringSize(Constants.LongCaliber2, caliberField);
            AnsiConsole.WriteLine("");
        }
    }

    static void DisplayStringSize(string text, PdfLoadedTextBoxField textBoxField)
    {
        AnsiConsole.MarkupLineInterpolated($"Checking string [green]'{text}'[/] to see if it will fit within {nameof(PdfLoadedTextBoxField)} [yellow]'{textBoxField.Name}'[/]...");

        Console.WriteLine();
        Console.WriteLine($"TextBox Bounds:");
        Console.WriteLine($"  Width: {textBoxField.Bounds.Width}");
        Console.WriteLine($"  Height: {textBoxField.Bounds.Height}");


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
