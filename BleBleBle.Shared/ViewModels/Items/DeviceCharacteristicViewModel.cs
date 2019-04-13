using BleBleBle.Shared.Interfaces;
using GalaSoft.MvvmLight;
using Plugin.BLE.Abstractions.Contracts;

namespace BleBleBle.Shared.ViewModels.Items
{
    public class DeviceCharacteristicViewModel : ViewModelBase, IDeviceDetailsListItem
    {
        public ICharacteristic Characteristic { get; }

        public DeviceCharacteristicViewModel(ICharacteristic characteristic)
        {
            Characteristic = characteristic;
        }

        

    }
}