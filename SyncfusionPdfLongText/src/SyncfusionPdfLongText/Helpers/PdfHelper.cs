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

        OpenInDefaultViewer(pdfFilePath: pdfTemplateFilePath);
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
