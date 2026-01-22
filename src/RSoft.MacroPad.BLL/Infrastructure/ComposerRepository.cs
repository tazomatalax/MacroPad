namespace RSoft.MacroPad.BLL.Infrastructure;

/// <summary>
/// Provides cached access to report composers for different protocol types and versions.
/// </summary>
public sealed class ComposerRepository
{
    private readonly Dictionary<(ProtocolType Type, byte Version), ReportComposer> _cache = [];

    /// <summary>
    /// Gets or creates a report composer for the specified protocol type and version.
    /// </summary>
    /// <param name="type">The protocol type.</param>
    /// <param name="version">The firmware version.</param>
    /// <returns>A report composer instance.</returns>
    public ReportComposer Get(ProtocolType type, byte version)
    {
        var key = (type, version);

        if (_cache.TryGetValue(key, out var composer))
            return composer;

        composer = type == ProtocolType.Legacy
            ? new LegacyReportComposer(version)
            : new ExtendedReportComposer(version);

        _cache[key] = composer;
        return composer;
    }
}
