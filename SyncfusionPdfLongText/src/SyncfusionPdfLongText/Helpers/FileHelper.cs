namespace SyncfusionPdfLongText.Helpers;

public static class FileHelper
{
    public static void ThrowIfNotExists(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException(message: $"Could not find file '{filePath}'.", fileName: filePath);
        }
    }
}
