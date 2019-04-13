using AoLibs.Navigation.Core.Interfaces;
using BleBleBle.Domain.Enums;
using BleBleBle.Interfaces;
using GalaSoft.MvvmLight;

namespace BleBleBle.Shared.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly INavigationManager<PageIndex> _navigationManager;
        private readonly IPermissionsManager _permissionsManager;

        public MainViewModel(INavigationManager<PageIndex> navigationManager,
            IPermissionsManager permissionsManager)
        {
            _navigationManager = navigationManager;
            _permissionsManager = permissionsManager;
        }

        public void Initialize()
        {
            _navigationManager.Navigate(_permissionsManager.AreAllPermissionsGranted
                ? PageIndex.ScannerPage
                : PageIndex.PermissionsPage);
        }
    }
}
