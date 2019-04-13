using System;
using System.Collections.Generic;
using System.Text;
using BleBleBle.Shared.Interfaces;
using GalaSoft.MvvmLight;
using Plugin.BLE.Abstractions.Contracts;

namespace BleBleBle.Shared.ViewModels.Items
{
    public class DeviceServiceViewModel : ViewModelBase, IDeviceDetailsListItem
    {
        public IService Service { get; }

        public DeviceServiceViewModel(IService service)
        {
            Service = service;
        }
    }
}
