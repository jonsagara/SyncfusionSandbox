using System.Diagnostics;

namespace SyncfusionSmallGroupedCheckbox;

public static class ChromeHelper
{
    /// <summary>
    /// Launch the PDF in a new Chrome browser tab.
    /// </summary>
    public static void OpenPdfInBrowser(string outputFilePath)
    {
        ArgumentNullException.ThrowIfNull(outputFilePath);

        using Process process = new();
        process.StartInfo.UseShellExecute = true;
        process.StartInfo.FileName = "chrome";
        process.StartInfo.Arguments = outputFilePath;
        process.Start();
    }
}
