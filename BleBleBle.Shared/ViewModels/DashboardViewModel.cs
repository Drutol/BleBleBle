using AoLibs.Navigation.Core.Interfaces;
using BleBleBle.Domain.Enums;
using GalaSoft.MvvmLight;

namespace BleBleBle.Shared.ViewModels
{
    public class DashboardViewModel : ViewModelBase
    {
        private readonly INavigationManager<PageIndex> _navigationManager;

        public DashboardViewModel(INavigationManager<PageIndex> navigationManager)
        {
            _navigationManager = navigationManager;
        }
        
    }
}
