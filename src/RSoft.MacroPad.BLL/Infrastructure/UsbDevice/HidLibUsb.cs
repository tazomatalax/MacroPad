namespace RSoft.MacroPad.BLL.Infrastructure.UsbDevice;

/// <summary>
/// USB implementation using the HidLibrary for device communication.
/// </summary>
public sealed class HidLibUsb : UsbBase
{
    private readonly HidLib _hidLib = new();

    /// <summary>
    /// Gets or sets the HID path fragment used to identify devices.
    /// </summary>
    public string? PathFragment { get; set; }

    /// <inheritdoc/>
    public override bool Write(Report report) => _hidLib.WriteDevice(report.ReportId, report.Data);

    /// <inheritdoc/>
    protected override bool CheckIfConnectedInternal() =>
        IsConnected = _hidLib.DeviceStatus && _hidLib.CheckConnection();

    /// <inheritdoc/>
    protected override bool ConnectInternal()
    {
        if (!_hidLib.ConnectDevice(SupportedDevices.ToArray()))
        {
            IsConnected = false;
            return false;
        }

        ProductId = _hidLib.ProductId;
        VendorId = _hidLib.VendorId;
        ProtocolType = _hidLib.ProtocolType ?? ProtocolType.Extended;

        Connected();
        return IsConnected = true;
    }
}
