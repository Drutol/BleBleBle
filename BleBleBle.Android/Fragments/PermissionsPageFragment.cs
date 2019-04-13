using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AoLibs.Navigation.Android.Navigation;
using AoLibs.Navigation.Android.Navigation.Attributes;
using AoLibs.Utilities.Android;
using BleBleBle.Domain.Enums;
using BleBleBle.Shared.ViewModels;

namespace BleBleBle.Android.Fragments
{
    [NavigationPage(PageIndex.PermissionsPage, NavigationPageAttribute.PageProvider.Cached)]
    public class PermissionsPageFragment : FragmentBase<PermissionsViewModel>
    {
        public override int LayoutResourceId { get; } = Resource.Layout.permissions_page;

        protected override void InitBindings()
        {
            GrantPermissionsButton.SetOnClickCommand(ViewModel.AskForPermissionsCommand);
        }

        #region Views

        private Button _grantPermissionsButton;

        public Button GrantPermissionsButton => _grantPermissionsButton ?? (_grantPermissionsButton = FindViewById<Button>(Resource.Id.GrantPermissionsButton));

        #endregion
    }
}