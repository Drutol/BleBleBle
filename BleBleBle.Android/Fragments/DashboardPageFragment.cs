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
using BleBleBle.Domain.Enums;
using BleBleBle.Shared.ViewModels;

namespace BleBleBle.Android.Fragments
{
    [NavigationPage(PageIndex.DashboardPage, NavigationPageAttribute.PageProvider.Cached)]
    public class DashboardPageFragment : FragmentBase<DashboardViewModel>
    {
        public override int LayoutResourceId { get; } = Resource.Layout.dashboard_page;

        protected override void InitBindings()
        {

        }
    }
}