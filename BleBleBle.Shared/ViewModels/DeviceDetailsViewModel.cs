using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using AoLibs.Adapters.Core.Interfaces;
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
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;

namespace BleBleBle.Shared.ViewModels
{
    public class DeviceDetailsViewModel : ViewModelBase
    {
        private readonly INavigationManager<PageIndex> _navigationManager;
        private readonly IAdapter _adapter;
        private readonly IMessageBoxProvider _messageBoxProvider;
        private IDevice _bluetoothDevice;
        private ScannedDevice _scannedDevice;

        public ScannedDevice ScannedDevice
        {
            get => _scannedDevice;
            set
            {
                _scannedDevice = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<IDeviceDetailsListItem> DeviceDetails { get; } =
            new ObservableCollection<IDeviceDetailsListItem>();

        public DeviceDetailsViewModel(INavigationManager<PageIndex> navigationManager,
            IAdapter adapter, IMessageBoxProvider messageBoxProvider)
        {
            _navigationManager = navigationManager;
            _adapter = adapter;
            _messageBoxProvider = messageBoxProvider;
            _navigationManager.WentBack += NavigationManagerOnNavigated;
        }

        private async void NavigationManagerOnNavigated(object sender, PageIndex e)
        {
            if (e == PageIndex.ScannerPage)
                await _adapter.DisconnectDeviceAsync(ScannedDevice.Device);
        }

        public async void NavigatedTo(DeviceDetailsNavArgs detailsNavArgs)
        {
            DeviceDetails.Clear();
            ScannedDevice = detailsNavArgs.ScannedDevice;

            using (_messageBoxProvider.ObtainLoaderLifetime($"Connecting to {ScannedDevice.AdvertisedName}", null))
            {
                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                try
                {
                    _bluetoothDevice = await _adapter.ConnectToKnownDeviceAsync(ScannedDevice.Guid,
                        new ConnectParameters(true, true), cts.Token);
                }
                catch (OperationCanceledException)
                {
                    await _messageBoxProvider.ShowMessageBoxOkAsync("Error", "Failed to connect to the device.", "Ok");
                    _navigationManager.GoBack();
                    return;
                }
            }

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
