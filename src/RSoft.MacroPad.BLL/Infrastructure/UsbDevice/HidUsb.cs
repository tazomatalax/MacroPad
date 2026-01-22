using System;
using HID;
using RSoft.MacroPad.BLL.Infrastructure.Protocol;

namespace RSoft.MacroPad.BLL.Infrastructure.UsbDevice
{
    public class HidUsb : UsbBase
    {
        Hid _hid = new Hid();
        IntPtr _hidPtr;

        public override bool Write(Report report)
        {
            return _hid.Write(new HidReport(report.ReportId, report.Data)) == Hid.HID_RETURN.SUCCESS;
        }

        protected override bool CheckIfConnectedInternal()
        {
            return IsConnected = _hid.Opened;
        }

        protected override bool ConnectInternal()
        {
            foreach (var dev in SupportedDevices)
            {
                if ((int)(_hidPtr = _hid.OpenDevice(dev.VendorId, dev.ProductId)) != -1)
                {
                    VendorId = dev.VendorId;
                    ProductId = dev.ProductId;

                    ProtocolType = dev.ProtocolType;

                    Connected();
                    return IsConnected = true;
                }
            }
            return IsConnected = false;
        }
    }
}
