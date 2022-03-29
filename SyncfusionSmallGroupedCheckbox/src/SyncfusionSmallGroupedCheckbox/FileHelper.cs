namespace SyncfusionSmallGroupedCheckbox;

public static class FileHelper
{
    public static string GetOutputFilePath(string inputFilePath, string type, string outputFolder)
    {
        ArgumentNullException.ThrowIfNull(inputFilePath);
        ArgumentNullException.ThrowIfNull(outputFolder);

        var fileNameNoExtension = Path.GetFileNameWithoutExtension(inputFilePath);
        var extension = Path.GetExtension(inputFilePath);

        // Ensure the directory exists.
        Directory.CreateDirectory(outputFolder);

        return Path.Combine(outputFolder, $"{fileNameNoExtension}_{type}_filled{extension}");
    }
}
