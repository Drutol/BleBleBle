using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AoLibs.Adapters.Android.Recycler;
using AoLibs.Navigation.Android.Navigation;
using AoLibs.Navigation.Android.Navigation.Attributes;
using AoLibs.Utilities.Android;
using BleBleBle.Domain.Enums;
using BleBleBle.Shared.ViewModels;
using BleBleBle.Shared.ViewModels.Items;
using GalaSoft.MvvmLight.Helpers;

namespace BleBleBle.Android.Fragments
{
    [NavigationPage(PageIndex.ScannerPage, NavigationPageAttribute.PageProvider.Cached)]
    public class ScannerPageFragment : FragmentBase<ScannerPageViewModel>
    {
        public ScannerPageFragment() : base(true)
        {

        }

        public override int LayoutResourceId { get; } = Resource.Layout.scanner_page;

        protected override void InitBindings()
        {
            RecyclerView.SetAdapter(
                new ObservableRecyclerAdapter<ScannedDeviceViewModel, ScannedDeviceViewHolder>(
                        ViewModel.ScannedDeviceViewModels, DataTemplate, ItemTemplate)
                    {StretchContentHorizonatally = true});

            Bindings.Add(this.SetBinding(() => ViewModel.EmptyNoticeVisibility).WhenSourceChanges(() =>
            {
                if (ViewModel.EmptyNoticeVisibility)
                {
                    EmptyView.Visibility = ViewStates.Visible;
                }
                else
                {
                    EmptyView.Visibility = ViewStates.Gone;
                }
            }));

            RecyclerView.SetLayoutManager(new LinearLayoutManager(Activity));
        }

        private View ItemTemplate(int viewtype)
        {
            return LayoutInflater.Inflate(Resource.Layout.item_scanned_device, null);
        }

        private void DataTemplate(ScannedDeviceViewModel item, ScannedDeviceViewHolder holder, int position)
        {
            holder.DeviceNameLabel.Text = item.ScannedDevice.AdvertisedName;
            holder.DeviceAddressLabel.Text = item.ScannedDevice.MacAddress;

            holder.ClickSurface.SetOnClickCommand(ViewModel.NavigateDeviceDetailsCommand, item);
        }

        public override void NavigatedTo()
        {
            ViewModel.NavigatedTo();
        }

        public override void NavigatedFrom()
        {
            ViewModel.NavigatedFrom();
        }

        #region Views

        private RecyclerView _recyclerView;
        private LinearLayout _emptyView;

        public RecyclerView RecyclerView => _recyclerView ?? (_recyclerView = FindViewById<RecyclerView>(Resource.Id.RecyclerView));
        public LinearLayout EmptyView => _emptyView ?? (_emptyView = FindViewById<LinearLayout>(Resource.Id.EmptyView));

        #endregion

        class ScannedDeviceViewHolder : BindingViewHolderBase<ScannedDeviceViewModel>
        {
            private readonly View _view;

            public ScannedDeviceViewHolder(View view) : base(view)
            {
                _view = view;
            }

            protected override void SetBindings()
            {
                Bindings.Add(this.SetBinding(() => ViewModel.SignalStrength).WhenSourceChanges(() =>
                    {
                        RssiProgressBar.Progress = 130 - Math.Abs(ViewModel.SignalStrength);
                        RssiValueLabel.Text = ViewModel.SignalStrength.ToString();
                    }));

            }

            private TextView _deviceNameLabel;
            private TextView _deviceAddressLabel;
            private ProgressBar _rssiProgressBar;
            private TextView _rssiValueLabel;
            private LinearLayout _clickSurface;

            public TextView DeviceNameLabel => _deviceNameLabel ?? (_deviceNameLabel = _view.FindViewById<TextView>(Resource.Id.DeviceNameLabel));
            public TextView DeviceAddressLabel => _deviceAddressLabel ?? (_deviceAddressLabel = _view.FindViewById<TextView>(Resource.Id.DeviceAddressLabel));
            public ProgressBar RssiProgressBar => _rssiProgressBar ?? (_rssiProgressBar = _view.FindViewById<ProgressBar>(Resource.Id.RssiProgressBar));
            public TextView RssiValueLabel => _rssiValueLabel ?? (_rssiValueLabel = _view.FindViewById<TextView>(Resource.Id.RssiValueLabel));
            public LinearLayout ClickSurface => _clickSurface ?? (_clickSurface = _view.FindViewById<LinearLayout>(Resource.Id.ClickSurface));
        }

    }
}
