using Syncfusion.Pdf.Parsing;

namespace SyncfusionSmallGroupedCheckbox;

public class SyncfusionHelper
{
    public static void SetGroupedCheckbox(string pdfTemplateFilePath, string outputFolder)
    {
        ArgumentNullException.ThrowIfNull(pdfTemplateFilePath);
        ArgumentNullException.ThrowIfNull(outputFolder);

        if (!File.Exists(pdfTemplateFilePath))
        {
            throw new FileNotFoundException($"Unable to load PDF template file '{pdfTemplateFilePath}'.");
        }

        // Load the template into a memory stream that we can modify.
        using (MemoryStream pdfTemplateStream = new())
        {
            using (var pdfTemplateFileStream = File.OpenRead(pdfTemplateFilePath))
            {
                pdfTemplateFileStream.CopyTo(pdfTemplateStream);
                pdfTemplateStream.Position = 0L;
            }


            //
            // Set the first grouped checkbox.
            //

            // Create a PDF document for filling form fields.
            using var pdfDocToFill = new PdfLoadedDocument(file: pdfTemplateStream);

            SetGroupedCheckbox(pdfDocToFill, fieldName: "firearmApp", selectedIndex: 0, isChecked: true);
            SetCheckbox(pdfDocToFill, fieldName: "Pawn", isChecked: true);
            SetGroupedCheckbox(pdfDocToFill, fieldName: "ApplicationTime", selectedIndex: 1, isChecked: true);
            SetCheckbox(pdfDocToFill, fieldName: "PrivateTrans", isChecked: true);
            SetGroupedCheckbox(pdfDocToFill, fieldName: "Condition", selectedIndex: 0, isChecked: true);

            SetGroupedCheckbox(pdfDocToFill, fieldName: "sex", selectedIndex: 0, isChecked: true);
            SetGroupedCheckbox(pdfDocToFill, fieldName: "citizen", selectedIndex: 0, isChecked: true);
            SetCheckbox(pdfDocToFill, fieldName: "RaceIndian", isChecked: true);
            SetCheckbox(pdfDocToFill, fieldName: "RaceAsian", isChecked: true);
            SetCheckbox(pdfDocToFill, fieldName: "RaceBlack", isChecked: true);
            SetCheckbox(pdfDocToFill, fieldName: "RaceHawaiian", isChecked: true);
            SetCheckbox(pdfDocToFill, fieldName: "RaceWhite", isChecked: true);


            //
            // Save the file to disk. Write to the specified output directory.
            //

            var outputFilePath = FileHelper.GetOutputFilePath(pdfTemplateFilePath, "syncfusion", outputFolder);
            SaveFile(pdfDocToFill, outputFilePath);


            //
            // Launch the PDF in a new Chrome browser tab.
            //

            ChromeHelper.OpenPdfInBrowser(outputFilePath);
        }
    }

    private static void SetGroupedCheckbox(PdfLoadedDocument pdfDoc, string fieldName, int selectedIndex, bool isChecked)
    {
        if (pdfDoc.Form.Fields.TryGetField(fieldName, out PdfLoadedField groupedField))
        {
            // We know this field is a grouped checkbox. Set the first child checkbox.
            var checkBoxField = (PdfLoadedCheckBoxField)groupedField;
            checkBoxField.Items[selectedIndex].Checked = isChecked;
        }
    }

    private static void SetCheckbox(PdfLoadedDocument pdfDoc, string fieldName, bool isChecked)
    {
        if (pdfDoc.Form.Fields.TryGetField(fieldName, out PdfLoadedField nonGroupedField))
        {
            // We know this field is NOT a grouped checkbox. Set the checkbox itself.
            var checkBoxField = (PdfLoadedCheckBoxField)nonGroupedField;
            checkBoxField.Checked = isChecked;
        }
    }

    private static void SaveFile(PdfLoadedDocument pdfDocument, string outputFilePath)
    {
        using (var generatedPdfStream = File.Open(outputFilePath, FileMode.Create, FileAccess.Write))
        {
            pdfDocument.Save(generatedPdfStream);
        }
    }
}
