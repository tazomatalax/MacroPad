using RSoft.MacroPad.BLL.Infrastructure.Configuration;

namespace RSoft.MacroPad.BLL.Infrastructure.UsbDevice;

/// <summary>
/// Provides sample/default device configurations for known macro pad devices.
/// </summary>
public static class DeviceSample
{
    /// <summary>
    /// The default vendor ID for supported devices.
    /// </summary>
    public const ushort DefaultVendorId = 4489;

    /// <summary>
    /// Gets the collection of known supported devices.
    /// </summary>
    public static IReadOnlyList<SupportedDevice> Devices { get; } =
    [
        new(4489, 34960, "mi_01", ProtocolType.Legacy),
        new(4489, 34864, "mi_00", ProtocolType.Extended),
        new(4489, 34865, "mi_00", ProtocolType.Extended),
        new(4489, 34866, "mi_00", ProtocolType.Extended),
        new(4489, 34967, "mi_00", ProtocolType.Extended),
        new(4489, 34932, "mi_00", ProtocolType.Extended)
    ];
}
