namespace RSoft.MacroPad.BLL.Infrastructure.Configuration;

/// <summary>
/// Represents the application configuration for supported HID devices.
/// </summary>
public sealed class Configuration
{
    /// <summary>
    /// Gets or sets the collection of supported macro pad devices.
    /// </summary>
    public IEnumerable<SupportedDevice> SupportedDevices { get; set; } = [];
}

/// <summary>
/// Represents a supported macro pad device configuration.
/// </summary>
/// <param name="VendorId">The USB vendor ID of the device.</param>
/// <param name="ProductId">The USB product ID of the device.</param>
/// <param name="PathPattern">The HID path pattern to match the device.</param>
/// <param name="ProtocolType">The communication protocol type for the device.</param>
public readonly record struct SupportedDevice(
    ushort VendorId,
    ushort ProductId,
    string PathPattern,
    ProtocolType ProtocolType);
