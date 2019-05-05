using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AoLibs.Adapters.Core.Interfaces;
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
        private readonly IDispatcherAdapter _dispatcherAdapter;
        private readonly IBluetoothDeviceDataExtractor _bluetoothDeviceDataExtractor;
        private CancellationTokenSource _taskCancelationSource;

        public bool ShouldScan { get; set; }

        public ObservableCollection<ScannedDeviceViewModel> ScannedDeviceViewModels { get; } = new ObservableCollection<ScannedDeviceViewModel>();

        private Dictionary<Guid, DateTime> _spotTimes = new Dictionary<Guid, DateTime>();

        public ScannerPageViewModel(INavigationManager<PageIndex> navigationManager,
            IAdapter adapter,
            IDispatcherAdapter dispatcherAdapter,
            IBluetoothDeviceDataExtractor bluetoothDeviceDataExtractor)
        {
            _navigationManager = navigationManager;
            _adapter = adapter;
            _dispatcherAdapter = dispatcherAdapter;
            _bluetoothDeviceDataExtractor = bluetoothDeviceDataExtractor;

            _adapter.DeviceDiscovered += AdapterOnDeviceDiscovered;
            _adapter.DeviceConnectionLost += AdapterOnDeviceConnectionLost;

            _taskCancelationSource = new CancellationTokenSource();
            Task.Factory.StartNew(RestartScanning, _taskCancelationSource.Token);
        }

        

        private void AdapterOnDeviceConnectionLost(object sender, DeviceErrorEventArgs e)
        {
            
            //var device = ScannedDeviceViewModels.FirstOrDefault(model => model.ScannedDevice.Device.Id == e.Device.Id);

            //if (device != null)
            //{
            //    ScannedDeviceViewModels.Remove(device);
            //}
        }

        public void NavigatedTo()
        {
            ShouldScan = true;
            ScannedDeviceViewModels.Clear();
            _adapter.StartScanningForDevicesAsync();
        }

        private async void RestartScanning()
        {
            while (true)
            {
                if (ShouldScan)
                {
                    await _adapter.StopScanningForDevicesAsync();

                    _dispatcherAdapter.Run(() =>
                    {
                        foreach (var goneDevice in _spotTimes.Where(pair =>
                            DateTime.UtcNow - pair.Value > TimeSpan.FromSeconds(20)))
                        {
                            var device =
                                ScannedDeviceViewModels.FirstOrDefault(model => model.ScannedDevice.Device.Id == goneDevice.Key);
                            if (device != null)
                                ScannedDeviceViewModels.Remove(device);
                        }
                    });

                    if(ShouldScan)
                        await _adapter.StartScanningForDevicesAsync();
                }


                Thread.Sleep(1000);
            }
        }

        public void NavigatedFrom()
        {
            _adapter.StopScanningForDevicesAsync();
        }

        private void AdapterOnDeviceDiscovered(object sender, DeviceEventArgs e)
        {
            _spotTimes[e.Device.Id] = DateTime.UtcNow;

            var device = ScannedDeviceViewModels.FirstOrDefault(model => model.ScannedDevice.Device.Id == e.Device.Id);

            if (device != null)
            {
                device.SignalStrength = e.Device.Rssi;
            }
            else
            {
                using (var scope = ResourceLocator.ObtainScope())
                {
                    ScannedDeviceViewModels.Add(scope.TypedResolve<ScannedDeviceViewModel>(new ScannedDevice
                    {
                        Device = e.Device,
                        Guid = e.Device.Id,
                        MacAddress = _bluetoothDeviceDataExtractor.GetMacAddressFromDevice(e.Device),
                        AdvertisedName = e.Device.Name,
                        SignalStrength = e.Device.Rssi
                    }));
                }
            }


        }

        public RelayCommand<ScannedDeviceViewModel> NavigateDeviceDetailsCommand =>
            new RelayCommand<ScannedDeviceViewModel>(async device =>
            {
                ShouldScan = false;
                await _adapter.StopScanningForDevicesAsync();
                _navigationManager.Navigate(PageIndex.DeviceDetailsPage, new DeviceDetailsNavArgs
                {
                    ScannedDevice = device.ScannedDevice
                });
            });

    }
}
