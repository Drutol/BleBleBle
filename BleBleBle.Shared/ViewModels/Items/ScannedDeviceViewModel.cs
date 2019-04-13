using System;
using System.Collections.Generic;
using System.Text;
using BleBleBle.Domain.Bluetooth;
using GalaSoft.MvvmLight;

namespace BleBleBle.Shared.ViewModels.Items
{
    public class ScannedDeviceViewModel : ViewModelBase
    {
        public ScannedDevice ScannedDevice { get; }

        public ScannedDeviceViewModel(ScannedDevice device)
        {
            ScannedDevice = device;
        }

        public int SignalStrength
        {
            get => ScannedDevice.SignalStrength;
            set
            {
                ScannedDevice.SignalStrength = value;
                RaisePropertyChanged();
            }
        }
    }
}
