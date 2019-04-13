using System;

namespace BleBleBle.Domain.Bluetooth
{
    public class ScannedDevice
    {
        public int SignalStrength { get; set; }
        public string AdvertisedName { get; set; }
        public string MacAddress { get; set; }
        public Guid Guid { get; set; }
    }
}
