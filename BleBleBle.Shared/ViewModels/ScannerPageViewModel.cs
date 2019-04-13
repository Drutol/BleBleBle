using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using AoLibs.Navigation.Core.Interfaces;
using BleBleBle.Domain.Bluetooth;
using BleBleBle.Domain.Enums;
using BleBleBle.Interfaces;
using BleBleBle.Shared.NavArgs;
using BleBleBle.Shared.Statics;
using BleBleBle.Shared.Utils;
using BleBleBle.Shared.ViewModels.Items;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;

namespace BleBleBle.Shared.ViewModels
{
    public class ScannerPageViewModel : ViewModelBase
    {
        private readonly INavigationManager<PageIndex> _navigationManager;
        private readonly IAdapter _adapter;
        private readonly IBluetoothDeviceDataExtractor _bluetoothDeviceDataExtractor;

        public ObservableCollection<ScannedDeviceViewModel> ScannedDeviceViewModels { get; } = new ObservableCollection<ScannedDeviceViewModel>();

        public ScannerPageViewModel(INavigationManager<PageIndex> navigationManager,
            IAdapter adapter,
            IBluetoothDeviceDataExtractor bluetoothDeviceDataExtractor)
        {
            _navigationManager = navigationManager;
            _adapter = adapter;
            _bluetoothDeviceDataExtractor = bluetoothDeviceDataExtractor;

            _adapter.DeviceDiscovered += AdapterOnDeviceDiscovered;
        }

        public void NavigatedTo()
        {
            ScannedDeviceViewModels.Clear();
            _adapter.StartScanningForDevicesAsync();
        }

        public void NavigatedFrom()
        {
            _adapter.StopScanningForDevicesAsync();
        }

        private void AdapterOnDeviceDiscovered(object sender, DeviceEventArgs e)
        {
            using (var scope = ResourceLocator.ObtainScope())
            {
                ScannedDeviceViewModels.Add(scope.TypedResolve<ScannedDeviceViewModel>(new ScannedDevice
                {
                    Guid = e.Device.Id,     
                    MacAddress = _bluetoothDeviceDataExtractor.GetMacAddressFromDevice(e.Device),
                    AdvertisedName = e.Device.Name,
                    SignalStrength = e.Device.Rssi
                }));
            }
        }

        public RelayCommand<ScannedDeviceViewModel> NavigateDeviceDetailsCommand =>
            new RelayCommand<ScannedDeviceViewModel>(device =>
            {
                _navigationManager.Navigate(PageIndex.DeviceDetailsPage, new DeviceDetailsNavArgs
                {
                    ScannedDevice = device.ScannedDevice
                });
            });

    }
}
