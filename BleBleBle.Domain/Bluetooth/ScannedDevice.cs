using System;
using Plugin.BLE.Abstractions.Contracts;

namespace BleBleBle.Domain.Bluetooth
{
    public class ScannedDevice
    {
        public int SignalStrength { get; set; }
        public string AdvertisedName { get; set; }
        public string MacAddress { get; set; }
        public Guid Guid { get; set; }
        public IDevice Device { get; set; }
    }
}
