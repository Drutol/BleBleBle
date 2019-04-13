using System;
using System.Collections.Generic;
using System.Text;
using Plugin.BLE.Abstractions.Contracts;

namespace BleBleBle.Interfaces
{
    public interface IBluetoothDeviceDataExtractor
    {
        string GetMacAddressFromDevice(IDevice device);
    }
}
