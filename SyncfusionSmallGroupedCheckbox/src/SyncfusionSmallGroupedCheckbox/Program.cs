using SyncfusionSmallGroupedCheckbox;


// This is the folder where we will store the filled PDFs.
var outputFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "GroupedCheckboxFilledPdfs");

// This is the relative path to the template PDF that we will fill.
const string templateFilePath = @"templates\application.pdf";


//
// These methods will save the filled PDFs to disk and launch them in Chrome.
//

// Syncfusion.Pdf: the first checked box is very tiny.
SyncfusionHelper.SetGroupedCheckbox(pdfTemplateFilePath: templateFilePath, outputFolder: outputFolder);

// iTextSharp: the first checked box is normal size.
iTextSharpHelper.SetGroupedCheckbox(pdfTemplateFilePath: templateFilePath, outputFolder: outputFolder);
