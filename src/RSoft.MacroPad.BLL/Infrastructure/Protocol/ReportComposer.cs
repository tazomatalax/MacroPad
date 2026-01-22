using RSoft.MacroPad.BLL.Infrastructure.Protocol.Legacy;

namespace RSoft.MacroPad.BLL.Infrastructure.Protocol;

/// <summary>
/// Base class for report composers that generate HID reports for macro pad devices.
/// </summary>
public abstract class ReportComposer(byte reportId)
{
    /// <summary>
    /// Gets the report ID used by this composer.
    /// </summary>
    public byte ReportId { get; } = reportId;

    /// <summary>
    /// Creates reports for a key sequence function.
    /// </summary>
    public abstract IEnumerable<Report> Key(
        InputAction action,
        byte layerNo,
        ushort delay,
        IEnumerable<(KeyCode Key, Modifier Modifiers)> sequence);

    /// <summary>
    /// Creates reports for a media key function.
    /// </summary>
    public abstract IEnumerable<Report> Media(InputAction action, byte layerNo, MediaKey key);

    /// <summary>
    /// Creates reports for a mouse function.
    /// </summary>
    public abstract IEnumerable<Report> Mouse(InputAction action, byte layerNo, MouseButton func, Modifier modifiers);

    /// <summary>
    /// Creates reports for an LED mode configuration.
    /// </summary>
    public abstract IEnumerable<Report> Led(byte layerNo, LedMode mode, LedColor color);
}

/// <summary>
/// Report composer for devices using the legacy protocol.
/// </summary>
public sealed class LegacyReportComposer(byte reportId) : ReportComposer(reportId)
{
    private List<Report> InitKeyFunction(byte layerNo)
    {
        var result = new List<Report>();
        // Version 0 doesn't support layers
        if (ReportId != 0)
        {
            result.Add(LayerSelectionReport.Create(ReportId, layerNo));
        }
        return result;
    }

    private static List<Report> FinalizeKeyFunction(List<Report> reports, byte reportId)
    {
        reports.Add(WriteFlashReport.Create(reportId));
        return reports;
    }

    /// <inheritdoc/>
    public override IEnumerable<Report> Key(
        InputAction action,
        byte layerNo,
        ushort delay,
        IEnumerable<(KeyCode Key, Modifier Modifiers)> sequence)
    {
        var keySequence = sequence.Any()
            ? sequence
            : [(KeyCode.None, Modifier.None)];

        var result = InitKeyFunction(layerNo);
        result.AddRange(KeyFunctionReport.Create(ReportId, action, layerNo, keySequence));
        return FinalizeKeyFunction(result, ReportId);
    }

    /// <inheritdoc/>
    public override IEnumerable<Report> Media(InputAction action, byte layerNo, MediaKey key)
    {
        var result = InitKeyFunction(layerNo);
        result.Add(KeyFunctionReport.CreateMultimedia(ReportId, action, layerNo, key));
        return FinalizeKeyFunction(result, ReportId);
    }

    /// <inheritdoc/>
    public override IEnumerable<Report> Mouse(InputAction action, byte layerNo, MouseButton func, Modifier modifiers)
    {
        var result = InitKeyFunction(layerNo);
        result.Add(MouseFunctionReport.Create(ReportId, action, layerNo, func, modifiers));
        return FinalizeKeyFunction(result, ReportId);
    }

    /// <inheritdoc/>
    public override IEnumerable<Report> Led(byte layerNo, LedMode mode, LedColor color)
    {
        // Version 0 only supports modes 0-2
        if ((byte)mode > 2 && ReportId == 0)
            return [];

        return
        [
            LedFunctionReport.Create(ReportId, mode, color),
            WriteFlashReport.Create(ReportId, led: true)
        ];
    }
}

/// <summary>
/// Report composer for devices using the extended protocol.
/// </summary>
public sealed class ExtendedReportComposer(byte reportId) : ReportComposer(reportId)
{
    /// <inheritdoc/>
    public override IEnumerable<Report> Key(
        InputAction action,
        byte layerNo,
        ushort delay,
        IEnumerable<(KeyCode Key, Modifier Modifiers)> sequence) =>
        [ExtendedReport.CreateKey(ReportId, action, layerNo, sequence, delay)];

    /// <inheritdoc/>
    public override IEnumerable<Report> Led(byte layerNo, LedMode mode, LedColor color) =>
        [ExtendedReport.CreateLed(ReportId, layerNo, mode, color)];

    /// <inheritdoc/>
    public override IEnumerable<Report> Media(InputAction action, byte layerNo, MediaKey key) =>
        [ExtendedReport.CreateMedia(ReportId, action, layerNo, key)];

    /// <inheritdoc/>
    public override IEnumerable<Report> Mouse(InputAction action, byte layerNo, MouseButton func, Modifier modifiers) =>
        [ExtendedReport.CreateMouse(ReportId, action, layerNo, func, modifiers)];
}
