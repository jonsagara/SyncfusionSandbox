namespace SyncfusionPdfLongText.Helpers;

public static class TemplateHelper
{
    public static MemoryStream GetTemplateAsMemoryStream(string pdfTemplateFilePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pdfTemplateFilePath);
        FileHelper.ThrowIfNotExists(pdfTemplateFilePath);

        var pdfTemplateMemoryStream = new MemoryStream();

        using (var pdfTemplateFileStream = File.OpenRead(pdfTemplateFilePath))
        {
            pdfTemplateFileStream.CopyTo(pdfTemplateMemoryStream);

            pdfTemplateMemoryStream.Position = 0L;
        }

        return pdfTemplateMemoryStream;
    }
}
