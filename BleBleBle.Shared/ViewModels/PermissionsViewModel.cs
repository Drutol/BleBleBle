using System;
using System.Collections.Generic;
using System.Text;
using AoLibs.Navigation.Core.Interfaces;
using BleBleBle.Domain.Enums;
using BleBleBle.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace BleBleBle.Shared.ViewModels
{
    public class PermissionsViewModel : ViewModelBase
    {
        private readonly INavigationManager<PageIndex> _navigationManager;
        private readonly IPermissionsManager _permissionsManager;

        public PermissionsViewModel(INavigationManager<PageIndex> navigationManager,
            IPermissionsManager permissionsManager)
        {
            _navigationManager = navigationManager;
            _permissionsManager = permissionsManager;
        }

        public RelayCommand AskForPermissionsCommand => new RelayCommand(async () =>
        {
            if (await _permissionsManager.AskForPermissionGrants())
            {
                _navigationManager.Navigate(PageIndex.ScannerPage);
            }
        });
    }
}
