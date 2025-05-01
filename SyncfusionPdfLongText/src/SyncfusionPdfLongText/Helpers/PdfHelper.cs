using System.Diagnostics;
using System.Runtime.InteropServices;
using Spectre.Console;
using Syncfusion.Drawing;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Parsing;

namespace SyncfusionPdfLongText.Helpers;

public static class PdfHelper
{
    private const string LongText1 = "longunbrokencalibertextohmygoshwhyisthissolong";
    private const string LongText2 = "9MM PARABULLUM";

    public static void ShowIfTextWillFitInTextBoxes(string pdfTemplateFilePath, string renderedPdfPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pdfTemplateFilePath);
        FileHelper.ThrowIfNotExists(pdfTemplateFilePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(renderedPdfPath);


        DisplayHeader("Show TextBox Layouter Results");
        AnsiConsole.WriteLine();

        AnsiConsole.WriteLine($"Loading PDF template file '{pdfTemplateFilePath}' into memory...");
        using var pdfTemplateStream = GetTemplateAsMemoryStream(pdfTemplateFilePath);
        using var pdfDocument = new PdfLoadedDocument(file: pdfTemplateStream);
        AnsiConsole.WriteLine("Done.");
        AnsiConsole.WriteLine();

        DescribeCaliberTextBoxFields(pdfDocument);
        AnsiConsole.WriteLine();
    }

    public static void FillPdfAndOpen(string pdfTemplateFilePath, string renderedPdfPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pdfTemplateFilePath);
        FileHelper.ThrowIfNotExists(pdfTemplateFilePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(renderedPdfPath);


        DisplayHeader("Filling the two problematic Caliber fields -AND- opening the filled PDF");
        AnsiConsole.WriteLine();


        PdfLoadedDocument? pdfDocument = null;

        using (var pdfTemplateStream = GetTemplateAsMemoryStream(pdfTemplateFilePath))
        {
            pdfDocument = new PdfLoadedDocument(file: pdfTemplateStream);

            FillTop2CaliberFields(pdfDocument, longCaliber1: LongText1, longCaliber2: LongText2);

            // Flatten the form fields so that they can no longer be filled.
            pdfDocument.Form.Flatten = true;

            // Save the filled form field to a file stream.
            using (var filledFileStream = File.OpenWrite(renderedPdfPath))
            {
                pdfDocument.Save(filledFileStream);
                pdfDocument.Close();
            }
        }

        OpenInDefaultViewer(pdfFilePath: renderedPdfPath);
    }


    //
    // Private methods
    //

    private static void DisplayHeader(string text)
    {
        AnsiConsole.MarkupLineInterpolated($"[blue]*** ============================================================================[/]");
        AnsiConsole.MarkupLineInterpolated($"[blue]    {text}[/]");
        AnsiConsole.MarkupLineInterpolated($"[blue]*** ============================================================================[/]");
    }

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

    private static void FillTop2CaliberFields(PdfLoadedDocument pdfDocument, string longCaliber1, string longCaliber2)
    {
        FillCaliberField(pdfDocument, fieldName: "topmostSubform[0].Page1[0].Q5_CaliberGauge_1", longCaliber: longCaliber1);
        FillCaliberField(pdfDocument, fieldName: "topmostSubform[0].Page1[0].Q5_CaliberGauge_2", longCaliber: longCaliber2);
    }

    private static void FillCaliberField(PdfLoadedDocument pdfDocument, string fieldName, string longCaliber)
    {
        if (pdfDocument.Form.Fields.TryGetField(fieldName: fieldName, out PdfLoadedField pdfLoadedField1))
        {
            var caliberLoadedTextBoxField = (PdfLoadedTextBoxField)pdfLoadedField1;

            // Fill the .Text property first. If the text doesn't fit, tell Syncfusion to auto-resize the text to fit.
            caliberLoadedTextBoxField.Text = longCaliber;

            if (IsTextClipped(longCaliber, caliberLoadedTextBoxField))
            {
                caliberLoadedTextBoxField.AutoResizeText = true;
            }
        }
    }

    /// <summary>
    /// Check to see if the text will fit in the text box. If not, make the text box an auto-size field.
    /// </summary>
    private static bool IsTextClipped(string? text, PdfLoadedTextBoxField textField)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        // The "layouter" determines whether the text will be clipped in the given textbox.
        var layouter = new PdfStringLayouter();
        var layoutResult = layouter.Layout(text, textField.Font, new PdfStringFormat(textField.TextAlignment), new SizeF(textField.Bounds.Width, textField.Bounds.Height));

        // If Remainder is a non-null, non-white space string, then there is clipped text, and
        //   we need to make the field auto-sized.
        return !string.IsNullOrWhiteSpace(layoutResult.Remainder);
    }

    private static void DescribeCaliberTextBoxFields(PdfLoadedDocument pdfDocument)
    {
        if (pdfDocument.Form.Fields.TryGetField("topmostSubform[0].Page1[0].Q5_CaliberGauge_1", out PdfLoadedField pdfField1))
        {
            var caliberField = (PdfLoadedTextBoxField)pdfField1;

            DisplayStringSize(LongText1, caliberField);
            AnsiConsole.WriteLine("");
        }

        if (pdfDocument.Form.Fields.TryGetField("topmostSubform[0].Page1[0].Q5_CaliberGauge_2", out PdfLoadedField pdfField2))
        {
            var caliberField = (PdfLoadedTextBoxField)pdfField2;

            DisplayStringSize(LongText2, caliberField);
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

    private static void OpenInDefaultViewer(string pdfFilePath)
    {
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = pdfFilePath,
                    UseShellExecute = true,
                });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", pdfFilePath);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", pdfFilePath);
            }
            else
            {
                throw new PlatformNotSupportedException("Unsupported OS platform.");
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLineInterpolated($"[red]Failed to open PDF file '{pdfFilePath}' in the system's default PDF viewer.[/]");
            AnsiConsole.WriteException(ex);
        }
    }
}
