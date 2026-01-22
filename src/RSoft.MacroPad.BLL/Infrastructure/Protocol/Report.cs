namespace RSoft.MacroPad.BLL.Infrastructure.Protocol;

/// <summary>
/// Represents a HID report to be sent to or received from a macro pad device.
/// </summary>
public class Report
{
    /// <summary>
    /// Gets the report ID for this HID report.
    /// </summary>
    public virtual byte ReportId { get; protected set; }

    /// <summary>
    /// Gets the raw data bytes of the report.
    /// </summary>
    public virtual byte[] Data { get; protected set; } = new byte[65];
}
