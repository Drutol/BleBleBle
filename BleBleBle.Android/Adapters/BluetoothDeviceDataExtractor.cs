using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BleBleBle.Interfaces;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Android;

namespace BleBleBle.Android.Adapters
{
    public class BluetoothDeviceDataExtractor : IBluetoothDeviceDataExtractor
    {
        public string GetMacAddressFromDevice(IDevice device)
        {
            return (device.NativeDevice as Device)?.BluetoothDevice.Address ?? "N/A";
        }
    }
}