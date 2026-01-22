using RSoft.MacroPad.BLL.Infrastructure.Configuration;

namespace RSoft.MacroPad.BLL.Infrastructure.UsbDevice;

/// <summary>
/// Defines the interface for USB communication with macro pad devices.
/// </summary>
public interface IUsb
{
    /// <summary>
    /// Gets a value indicating whether a device is currently connected.
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Gets or sets the USB vendor ID of the connected device.
    /// </summary>
    ushort VendorId { get; set; }

    /// <summary>
    /// Gets or sets the USB product ID of the connected device.
    /// </summary>
    ushort ProductId { get; set; }

    /// <summary>
    /// Gets the protocol type used by the connected device.
    /// </summary>
    ProtocolType ProtocolType { get; }

    /// <summary>
    /// Gets or sets the firmware version of the connected device.
    /// </summary>
    byte Version { get; set; }

    /// <summary>
    /// Gets or sets the collection of supported devices to search for.
    /// </summary>
    IEnumerable<SupportedDevice> SupportedDevices { get; set; }

    /// <summary>
    /// Checks if a device is currently connected.
    /// </summary>
    bool CheckIfConnected();

    /// <summary>
    /// Attempts to connect to a supported device.
    /// </summary>
    bool Connect();

    /// <summary>
    /// Writes a report to the connected device.
    /// </summary>
    bool Write(Report report);

    /// <summary>
    /// Occurs when a device is connected.
    /// </summary>
    event EventHandler OnConnected;
}

/// <summary>
/// Base class for USB communication implementations.
/// </summary>
public abstract class UsbBase : IUsb
{
    public ushort VendorId { get; set; }
    public ushort ProductId { get; set; }
    public ProtocolType ProtocolType { get; protected set; } = ProtocolType.Legacy;
    public byte Version { get; set; }

    public event EventHandler? OnConnected;

    public bool CheckIfConnected() => CheckIfConnectedInternal();

    public IEnumerable<SupportedDevice> SupportedDevices { get; set; } = DeviceSample.Devices;

    public bool IsConnected { get; protected set; }

    protected abstract bool CheckIfConnectedInternal();

    public bool Connect() => CheckIfConnectedInternal() || ConnectInternal();

    protected abstract bool ConnectInternal();

    protected virtual byte KeyBoardVersionCheck()
    {
        if (Write(VersionCheckReport.Create(0)))
            return Version = 0;
        if (Write(VersionCheckReport.Create(2)))
            return Version = 2;
        return Version = 3;
    }

    protected void Connected()
    {
        KeyBoardVersionCheck();
        OnConnected?.Invoke(this, EventArgs.Empty);
    }

    public abstract bool Write(Report report);
}
