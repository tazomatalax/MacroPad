using System.IO;

namespace RSoft.MacroPad.BLL;

/// <summary>
/// Provides logging functionality for HID communication diagnostics.
/// </summary>
public static class HidLog
{
    private const string LogFileName = "hid.log";

    /// <summary>
    /// Clears the HID log file.
    /// </summary>
    public static void ClearLog()
    {
        try
        {
            File.WriteAllText(LogFileName, string.Empty);
        }
        catch
        {
            // Silently ignore logging failures
        }
    }

    /// <summary>
    /// Appends a message to the HID log file.
    /// </summary>
    /// <param name="reportId">The HID report ID.</param>
    /// <param name="data">The data bytes to log.</param>
    public static void AppendMsg(byte reportId, IEnumerable<byte> data)
    {
        try
        {
            var logEntry = $"""
                {reportId}

                {string.Join(Environment.NewLine, data)}
                ------

                """;
            File.AppendAllText(LogFileName, logEntry);
        }
        catch
        {
            // Silently ignore logging failures
        }
    }
}
