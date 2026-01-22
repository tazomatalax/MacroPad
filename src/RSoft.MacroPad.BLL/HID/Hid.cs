using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

namespace HID;

public class Hid
{
    public const uint GENERIC_READ = 2147483648;
    public const uint GENERIC_WRITE = 1073741824;
    public const uint FILE_SHARE_READ = 1;
    public const uint FILE_SHARE_WRITE = 2;
    public const int OPEN_EXISTING = 3;
    private static readonly IntPtr INVALID_HANDLE_VALUE = new(-1);
    private const int MAX_USB_DEVICES = 64;
    private bool deviceOpened;
    private FileStream? hidDevice;
#pragma warning disable CS0414 // The field is assigned but its value is never used
    private IAsyncResult? readResult;
#pragma warning restore CS0414 // The field is assigned but its value is never used
    private int outputReportLength;
    private int inputReportLength;

    public int OutputReportLength => outputReportLength;

    public int InputReportLength => inputReportLength;

    public static HID_RETURN GetDeviceSerialList(
      ushort vID,
      ushort pID,
      ref List<string> serialList)
    {
        serialList.Clear();
        List<string> deviceList = [];
        GetHidDeviceList(ref deviceList);
        if (deviceList.Count == 0)
            return HID_RETURN.NO_DEVICE_CONECTED;
        for (int index = 0; index < deviceList.Count; ++index)
        {
            IntPtr file = CreateFile(deviceList[index], 3221225472U, 0U, 0U, 3U, 1073741824U, 0U);
            if (file != INVALID_HANDLE_VALUE)
            {
                IntPtr num = Marshal.AllocHGlobal(512);
                HidD_GetAttributes(file, out var attributes);
                HidD_GetSerialNumberString(file, num, 512);
                string? stringAuto = Marshal.PtrToStringAuto(num);
                Marshal.FreeHGlobal(num);
                if ((int)attributes.VendorID == (int)vID && (int)attributes.ProductID == (int)pID && stringAuto != null)
                    serialList.Add(stringAuto);
                CloseHandle(file);
            }
        }
        return HID_RETURN.SUCCESS;
    }

    public IntPtr OpenDevice(ushort vID, ushort pID)
    {
        if (deviceOpened)
            return INVALID_HANDLE_VALUE;
        List<string> deviceList = [];
        GetHidDeviceList(ref deviceList);
        if (deviceList.Count == 0)
            return INVALID_HANDLE_VALUE;
        for (int index = 0; index < deviceList.Count; ++index)
        {
            IntPtr file = CreateFile(deviceList[index], 3221225472U, 0U, 0U, 3U, 1073741824U, 0U);
            if (file != INVALID_HANDLE_VALUE)
            {
                IntPtr num = Marshal.AllocHGlobal(512);
                if (!HidD_GetAttributes(file, out var attributes))
                {
                    CloseHandle(file);
                    return INVALID_HANDLE_VALUE;
                }
                HidD_GetSerialNumberString(file, num, 512);
                Marshal.PtrToStringAuto(num);
                Marshal.FreeHGlobal(num);
                if ((int)attributes.VendorID == (int)vID && (int)attributes.ProductID == (int)pID)
                {
                    HidD_GetPreparsedData(file, out var PreparsedData);
                    HidP_GetCaps(PreparsedData, out var Capabilities);
                    HidD_FreePreparsedData(PreparsedData);
                    outputReportLength = Capabilities.OutputReportByteLength;
                    inputReportLength = Capabilities.InputReportByteLength;
                    hidDevice = new FileStream(new SafeFileHandle(file, false), FileAccess.ReadWrite, inputReportLength, true);
                    deviceOpened = true;
                    BeginAsyncRead();
                    return file;
                }
            }
            CloseHandle(file);
        }
        return INVALID_HANDLE_VALUE;
    }

    public bool Opened => deviceOpened;

    public void CloseDevice(IntPtr device)
    {
        if (!deviceOpened)
            return;
        deviceOpened = false;
        hidDevice?.Close();
        CloseHandle(device);
    }

    private void BeginAsyncRead()
    {
        if (hidDevice == null) return;
        byte[] numArray = new byte[InputReportLength];
        
        readResult = hidDevice.BeginRead(numArray, 0, InputReportLength, new AsyncCallback(ReadCompleted), numArray);
    }

    private void ReadCompleted(IAsyncResult iResult)
    {
        var asyncState = (byte[]?)iResult.AsyncState;
        try
        {
            if (!deviceOpened || hidDevice == null || asyncState == null)
                return;
            hidDevice.EndRead(iResult);
            byte[] arrayBuff = new byte[asyncState.Length - 1];
            for (int index = 1; index < asyncState.Length; ++index)
                arrayBuff[index - 1] = asyncState[index];
            OnDataReceived(new HidReport(asyncState[0], arrayBuff));
            BeginAsyncRead();
        }
        catch (IOException)
        {
            OnDeviceRemoved(EventArgs.Empty);
        }
    }

    public event EventHandler<HidReport>? DataReceived;

    protected virtual void OnDataReceived(HidReport e)
    {
        DataReceived?.Invoke(this, e);
    }

    public event EventHandler? DeviceRemoved;

    protected virtual void OnDeviceRemoved(EventArgs e)
    {
        deviceOpened = false;
        DeviceRemoved?.Invoke(this, e);
    }

    public HID_RETURN Write(HidReport r)
    {
        if (deviceOpened && hidDevice != null)
        {
            try
            {
                byte[] buffer = new byte[outputReportLength];
                buffer[0] = r.reportID;
                int num = r.reportBuff.Length >= outputReportLength - 1 ? outputReportLength - 1 : r.reportBuff.Length;
                for (int index = 1; index <= num; ++index)
                    buffer[index] = r.reportBuff[index - 1];
                hidDevice.Write(buffer, 0, 65);
                return HID_RETURN.SUCCESS;
            }
            catch
            {
            }
        }
        return HID_RETURN.WRITE_FAILD;
    }

    public static void GetHidDeviceList(ref List<string> deviceList)
    {
        Guid empty = Guid.Empty;
        deviceList.Clear();
        HidD_GetHidGuid(ref empty);
        IntPtr classDevs = SetupDiGetClassDevs(ref empty, 0U, IntPtr.Zero, DIGCF.DIGCF_PRESENT | DIGCF.DIGCF_DEVICEINTERFACE);
        if (classDevs != IntPtr.Zero)
        {
            SP_DEVICE_INTERFACE_DATA deviceInterfaceData = new SP_DEVICE_INTERFACE_DATA();
            deviceInterfaceData.cbSize = Marshal.SizeOf((object)deviceInterfaceData);
            for (uint memberIndex = 0; memberIndex < 64U; ++memberIndex)
            {
                if (SetupDiEnumDeviceInterfaces(classDevs, IntPtr.Zero, ref empty, memberIndex, ref deviceInterfaceData))
                {
                    int requiredSize = 0;
                    SetupDiGetDeviceInterfaceDetail(classDevs, ref deviceInterfaceData, IntPtr.Zero, requiredSize, ref requiredSize, null);
                    IntPtr num = Marshal.AllocHGlobal(requiredSize);
                    Marshal.StructureToPtr((object)new SP_DEVICE_INTERFACE_DETAIL_DATA()
                    {
                        cbSize = Marshal.SizeOf(typeof(SP_DEVICE_INTERFACE_DETAIL_DATA))
                    }, num, false);
                    if (SetupDiGetDeviceInterfaceDetail(classDevs, ref deviceInterfaceData, num, requiredSize, ref requiredSize, null))
                    {
                        string? str = Marshal.PtrToStringAuto((IntPtr)((int)num + 4));
                        if(str != null) deviceList.Add(str);
                    }
                    Marshal.FreeHGlobal(num);
                }
            }
        }
        SetupDiDestroyDeviceInfoList(classDevs);
    }

    [DllImport("hid.dll")]
    private static extern void HidD_GetHidGuid(ref Guid HidGuid);

    [DllImport("setupapi.dll", SetLastError = true)]
    private static extern IntPtr SetupDiGetClassDevs(
      ref Guid ClassGuid,
      uint Enumerator,
      IntPtr HwndParent,
      DIGCF Flags);

    [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool SetupDiDestroyDeviceInfoList(IntPtr deviceInfoSet);

    [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool SetupDiEnumDeviceInterfaces(
      IntPtr deviceInfoSet,
      IntPtr deviceInfoData,
      ref Guid interfaceClassGuid,
      uint memberIndex,
      ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);

    [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool SetupDiGetDeviceInterfaceDetail(
      IntPtr deviceInfoSet,
      ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData,
      IntPtr deviceInterfaceDetailData,
      int deviceInterfaceDetailDataSize,
      ref int requiredSize,
      SP_DEVINFO_DATA? deviceInfoData);

    [DllImport("hid.dll")]
    private static extern bool HidD_GetAttributes(
      IntPtr hidDeviceObject,
      out HIDD_ATTRIBUTES attributes);

    [DllImport("hid.dll")]
    private static extern bool HidD_GetSerialNumberString(
      IntPtr hidDeviceObject,
      IntPtr buffer,
      int bufferLength);

    [DllImport("hid.dll")]
    private static extern bool HidD_GetPreparsedData(
      IntPtr hidDeviceObject,
      out IntPtr PreparsedData);

    [DllImport("hid.dll")]
    private static extern bool HidD_FreePreparsedData(IntPtr PreparsedData);

    [DllImport("hid.dll")]
    private static extern uint HidP_GetCaps(IntPtr PreparsedData, out HIDP_CAPS Capabilities);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr CreateFile(
      string fileName,
      uint desiredAccess,
      uint shareMode,
      uint securityAttributes,
      uint creationDisposition,
      uint flagsAndAttributes,
      uint templateFile);

    [DllImport("kernel32.dll")]
    private static extern int CloseHandle(IntPtr hObject);

    [DllImport("Kernel32.dll", SetLastError = true)]
    private static extern bool ReadFile(
      IntPtr file,
      byte[] buffer,
      uint numberOfBytesToRead,
      out uint numberOfBytesRead,
      IntPtr lpOverlapped);

    [DllImport("Kernel32.dll", SetLastError = true)]
    private static extern bool WriteFile(
      IntPtr file,
      byte[] buffer,
      uint numberOfBytesToWrite,
      out uint numberOfBytesWritten,
      IntPtr lpOverlapped);

    [DllImport("User32.dll", SetLastError = true)]
    private static extern IntPtr RegisterDeviceNotification(
      IntPtr recipient,
      IntPtr notificationFilter,
      int flags);

    public static IntPtr RegisterHIDNotification(IntPtr recipient)
    {
        IntPtr num = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(DevBroadcastDeviceInterfaceBuffer)));
        Marshal.StructureToPtr((object)new DevBroadcastDeviceInterfaceBuffer(5), num, false);
        return RegisterDeviceNotification(recipient, num, 0);
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UnregisterDeviceNotification(IntPtr handle);

    public static bool UnRegisterHIDNotification(IntPtr hDEVNotify) => UnregisterDeviceNotification(hDEVNotify);

    public enum HID_RETURN
    {
        SUCCESS,
        NO_DEVICE_CONECTED,
        DEVICE_NOT_FIND,
        DEVICE_OPENED,
        WRITE_FAILD,
        READ_FAILD,
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct DevBroadcastDeviceInterfaceBuffer
    {
        [FieldOffset(0)]
        public int dbch_size;
        [FieldOffset(4)]
        public int dbch_devicetype;
        [FieldOffset(8)]
        public int dbch_reserved;

        public DevBroadcastDeviceInterfaceBuffer(int deviceType)
        {
            dbch_size = Marshal.SizeOf(typeof(DevBroadcastDeviceInterfaceBuffer));
            dbch_devicetype = deviceType;
            dbch_reserved = 0;
        }
    }
}
