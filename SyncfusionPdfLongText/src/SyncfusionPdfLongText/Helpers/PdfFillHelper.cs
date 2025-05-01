using System.Diagnostics;
using System.Runtime.InteropServices;
using Spectre.Console;
using Syncfusion.Drawing;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Parsing;

namespace SyncfusionPdfLongText.Helpers;

public static class PdfFillHelper
{
    public static void FillPdfAndOpen(string pdfTemplateFilePath, string renderedPdfPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pdfTemplateFilePath);
        FileHelper.ThrowIfNotExists(pdfTemplateFilePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(renderedPdfPath);


        DisplayHelper.Header("Filling the two problematic Caliber fields -AND- opening the filled PDF");
        AnsiConsole.WriteLine();


        PdfLoadedDocument? pdfDocument = null;

        using (var pdfTemplateStream = TemplateHelper.GetTemplateAsMemoryStream(pdfTemplateFilePath))
        {
            pdfDocument = new PdfLoadedDocument(file: pdfTemplateStream);

            AnsiConsole.WriteLine("Filling the two caliber fields with long strings...");
            FillTop2CaliberFields(pdfDocument, longCaliber1: Constants.LongCaliber1, longCaliber2: Constants.LongCaliber2);
            AnsiConsole.WriteLine("Done.");
            AnsiConsole.WriteLine();

            // Flatten the form fields so that they can no longer be filled.
            pdfDocument.Form.Flatten = true;

            // Save the filled form field to a file stream.
            AnsiConsole.MarkupLineInterpolated($"Saving the filled PDF to [blue]'{renderedPdfPath}'[/]...");
            using (var filledFileStream = File.OpenWrite(renderedPdfPath))
            {
                pdfDocument.Save(filledFileStream);
                pdfDocument.Close();
            }
            AnsiConsole.WriteLine("Done.");
            AnsiConsole.WriteLine();
        }

        AnsiConsole.WriteLine("Opening the filled PDF in the OS's default PDF viewer...");
        OpenInDefaultViewer(pdfFilePath: renderedPdfPath);
        AnsiConsole.WriteLine("Done.");
        AnsiConsole.WriteLine();
    }


    //
    // Private methods
    //

    private static void FillTop2CaliberFields(PdfLoadedDocument pdfDocument, string longCaliber1, string longCaliber2)
    {
        FillCaliberField(pdfDocument, fieldName: Constants.CaliberField1Name, longCaliber: longCaliber1);
        FillCaliberField(pdfDocument, fieldName: Constants.CaliberField2Name, longCaliber: longCaliber2);
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
