using HidLibrary;
using RSoft.MacroPad.BLL.Infrastructure.Configuration;

namespace RSoft.MacroPad.BLL.Infrastructure.UsbDevice;

/// <summary>
/// Provides HID library integration for communicating with macro pad devices.
/// </summary>
public sealed class HidLib
{
    private bool _deviceStatus;
    private readonly List<HidDevice> _deviceList = [];
    private HidDevice? _hidDevice;

    /// <summary>
    /// Gets the protocol type of the connected device.
    /// </summary>
    public ProtocolType? ProtocolType { get; private set; }

    /// <summary>
    /// Gets the USB vendor ID of the connected device.
    /// </summary>
    public ushort VendorId { get; private set; }

    /// <summary>
    /// Gets the USB product ID of the connected device.
    /// </summary>
    public ushort ProductId { get; private set; }

    /// <summary>
    /// Gets a value indicating whether a device is connected.
    /// </summary>
    public bool DeviceStatus => _deviceStatus;

    /// <summary>
    /// Attempts to connect to one of the supported devices.
    /// </summary>
    /// <param name="supportedProducts">The collection of supported devices to search for.</param>
    /// <returns>True if a device was found and connected; otherwise, false.</returns>
    public bool ConnectDevice(params SupportedDevice[] supportedProducts)
    {
        foreach (var supportedProduct in supportedProducts)
        {
            _hidDevice = HidDevices.Enumerate(supportedProduct.VendorId, supportedProduct.ProductId).FirstOrDefault();
            if (_hidDevice is null)
                continue;

            foreach (var hidDevice in HidDevices.Enumerate(supportedProduct.VendorId))
            {
                if (!hidDevice.DevicePath.Contains(supportedProduct.PathPattern, StringComparison.OrdinalIgnoreCase))
                    continue;

                _deviceList.Add(hidDevice);
                _hidDevice = hidDevice;
                _hidDevice.OpenDevice();

                ProtocolType = supportedProduct.ProtocolType;
                VendorId = supportedProduct.VendorId;
                ProductId = supportedProduct.ProductId;

                _deviceStatus = true;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Checks if the current device connection is still valid.
    /// </summary>
    /// <returns>True if the device is still connected; otherwise, false.</returns>
    public bool CheckConnection()
    {
        if (_hidDevice is null)
            return false;

        if (_hidDevice.IsConnected)
            return true;

        _hidDevice.CloseDevice();
        _deviceStatus = false;
        return false;
    }

    /// <summary>
    /// Writes a report to the connected device.
    /// </summary>
    /// <param name="reportId">The report ID to use.</param>
    /// <param name="buffer">The data buffer to write.</param>
    /// <returns>True if the write was successful; otherwise, false.</returns>
    public bool WriteDevice(byte reportId, byte[] buffer)
    {
        if (_hidDevice is null)
            return false;

        var report = _hidDevice.CreateReport();
        report.ReportId = reportId;

        var byteCount = report.Data.Length;
        for (var i = 0; i < byteCount; ++i)
            report.Data[i] = buffer[i];

        var dataToLog = ProtocolType == Model.ProtocolType.Legacy
            ? report.Data.Take(8)
            : report.Data;
        HidLog.AppendMsg(report.ReportId, dataToLog);

        return _hidDevice.WriteReport(report, 500);
    }
}
