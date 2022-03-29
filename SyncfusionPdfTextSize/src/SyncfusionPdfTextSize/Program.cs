
using Syncfusion.Drawing;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Parsing;

const string PdfTemplateFilePath = @"templates\form.pdf";

using var pdfTemplateStream = File.OpenRead(PdfTemplateFilePath);
using var pdfDocument = new PdfLoadedDocument(pdfTemplateStream);

var dateString = new DateTime(2021, 11, 18).ToString("MM/dd/yyyy");

if (pdfDocument.Form.Fields.TryGetField("topmostSubform[0].Page1[0].Transferdate1[0]", out PdfLoadedField dispDateField))
{
    if (dispDateField is PdfLoadedTextBoxField textField)
    {
        CheckStringSize(dateString, textField);
    }
}

static void CheckStringSize(string text, PdfLoadedTextBoxField textField)
{
    Console.WriteLine($"***");
    Console.WriteLine($"*** Checking string '{text}' to see if it will fit within {nameof(PdfLoadedTextBoxField)} '{textField.Name}'");
    Console.WriteLine($"***");


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
        Console.WriteLine("***");
        Console.WriteLine($"*** Syncfusion.Pdf reports that the string '{text}' WILL fit within the TextBoxField.");
        Console.WriteLine("***");
    }
    else
    {
        Console.WriteLine("***");
        Console.WriteLine($"*** Syncfusion.Pdf reports that the string '{text}' will NOT fit within the TextBoxField.");
        Console.WriteLine("***");
    }
}

