using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using AoLibs.Navigation.Core.Interfaces;
using BleBleBle.Domain.Bluetooth;
using BleBleBle.Domain.Enums;
using BleBleBle.Shared.Interfaces;
using BleBleBle.Shared.NavArgs;
using BleBleBle.Shared.Statics;
using BleBleBle.Shared.Utils;
using BleBleBle.Shared.ViewModels.Items;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Plugin.BLE.Abstractions.Contracts;

namespace BleBleBle.Shared.ViewModels
{
    public class DeviceDetailsViewModel : ViewModelBase
    {
        private readonly INavigationManager<PageIndex> _navigationManager;
        private readonly IAdapter _adapter;
        private IDevice _bluetoothDevice;

        public ScannedDevice Device { get; set; }

        public ObservableCollection<IDeviceDetailsListItem> DeviceDetails { get; } =
            new ObservableCollection<IDeviceDetailsListItem>();

        public DeviceDetailsViewModel(INavigationManager<PageIndex> navigationManager,
            IAdapter adapter)
        {
            _navigationManager = navigationManager;
            _adapter = adapter;
        }

        public async void NavigatedTo(DeviceDetailsNavArgs detailsNavArgs)
        {
            DeviceDetails.Clear();
            Device = detailsNavArgs.ScannedDevice;

            _bluetoothDevice = await _adapter.ConnectToKnownDeviceAsync(Device.Guid);

            using (var scope = ResourceLocator.ObtainScope())
            {
                var services = await _bluetoothDevice.GetServicesAsync();
                foreach (var service in services)
                {
                    DeviceDetails.Add(scope.TypedResolve<DeviceServiceViewModel, IService>(service));

                    var characteristics = await service.GetCharacteristicsAsync();

                    foreach (var characteristic in characteristics)
                    {
                        DeviceDetails.Add(scope.TypedResolve<DeviceCharacteristicViewModel, ICharacteristic>(characteristic));
                    }
                }
            }
        }

        public RelayCommand<DeviceCharacteristicViewModel> NavigateCharacteristicDetailsCommand =>
            new RelayCommand<DeviceCharacteristicViewModel>(characteristic =>
            {
                _navigationManager.Navigate(PageIndex.CharacteristicDetailsPage, new DeviceCharacteristicsDetailsNavArgs
                {
                    Characteristic = characteristic.Characteristic
                });
            });

    }
}
