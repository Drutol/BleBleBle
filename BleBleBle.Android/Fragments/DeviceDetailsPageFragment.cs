﻿using System;
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
using AoLibs.Utilities.Android.Listeners;
using BleBleBle.Domain.Enums;
using BleBleBle.Shared.Interfaces;
using BleBleBle.Shared.NavArgs;
using BleBleBle.Shared.ViewModels;
using BleBleBle.Shared.ViewModels.Items;
using Plugin.BLE.Abstractions.Contracts;
using GalaSoft.MvvmLight.Helpers;

namespace BleBleBle.Android.Fragments
{
    [NavigationPage(PageIndex.DeviceDetailsPage, NavigationPageAttribute.PageProvider.Cached)]
    public class DeviceDetailsPageFragment : FragmentBase<DeviceDetailsViewModel>
    {
        public override int LayoutResourceId { get; } = Resource.Layout.device_details_page;

        protected override void InitBindings()
        {
            Bindings.Add(this.SetBinding(() => ViewModel.ScannedDevice).WhenSourceChanges(() =>
            {
                if(ViewModel.ScannedDevice == null)
                    return;

                DeviceLabel.Text = ViewModel.ScannedDevice.AdvertisedName;
            }));

            RecyclerView.SetAdapter(
                new ObservableRecyclerAdapterWithMultipleViewTypes<IDeviceDetailsListItem, RecyclerView.ViewHolder>(
                    new Dictionary<Type, ObservableRecyclerAdapterWithMultipleViewTypes<IDeviceDetailsListItem,
                        RecyclerView.ViewHolder>.IItemEntry>
                    {
                        {
                            typeof(DeviceServiceViewModel),
                            new ObservableRecyclerAdapterWithMultipleViewTypes<IDeviceDetailsListItem,
                                RecyclerView.ViewHolder>.SpecializedItemEntry<DeviceServiceViewModel, ServiceViewHolder>
                            {
                                ItemTemplate = type => LayoutInflater.Inflate(Resource.Layout.item_ble_service, null),
                                SpecializedDataTemplate = ServiceDataTemplate
                            }
                        },
                        {
                            typeof(DeviceCharacteristicViewModel),
                            new ObservableRecyclerAdapterWithMultipleViewTypes<IDeviceDetailsListItem,
                                RecyclerView.ViewHolder>.SpecializedItemEntry<DeviceCharacteristicViewModel,
                                CharacteristicViewHolder>
                            {
                                ItemTemplate = type => LayoutInflater.Inflate(Resource.Layout.item_ble_characteristic, null),
                                SpecializedDataTemplate = CharacteristicDataTemplate
                            }
                        }
                    }, ViewModel.DeviceDetails) {StretchContentHorizonatally = true});
            RecyclerView.SetLayoutManager(new LinearLayoutManager(Activity));


        }

        public override void NavigatedTo()
        {
            ViewModel.NavigatedTo(NavigationArguments as DeviceDetailsNavArgs);
        }

        public override void NavigatedFrom()
        {

        }

        private void CharacteristicDataTemplate(DeviceCharacteristicViewModel item, CharacteristicViewHolder holder, int position)
        {
            holder.CharacteristicNameLabel.Text = item.Characteristic.Name;
            holder.CharacteristicUuidLabel.Text = item.Characteristic.Uuid;
            holder.IconCanRead.Visibility = item.Characteristic.CanRead ? ViewStates.Visible : ViewStates.Gone;
            holder.IconCanWrite.Visibility = item.Characteristic.CanWrite  ? ViewStates.Visible : ViewStates.Gone;
            holder.IconCanNotify.Visibility = item.Characteristic.CanUpdate ? ViewStates.Visible : ViewStates.Gone;

            holder.ClickSurface.SetOnClickCommand(ViewModel.NavigateCharacteristicDetailsCommand, item);
        }

        private void ServiceDataTemplate(DeviceServiceViewModel item, ServiceViewHolder holder, int position)
        {
            holder.ServiceNameLabel.Text = item.Service.Name;
            holder.ServiceUuidLabel.Text = item.Service.Id.ToString();
        }

        #region Views

        private TextView _deviceLabel;
        private RecyclerView _recyclerView;

        public TextView DeviceLabel => _deviceLabel ?? (_deviceLabel = FindViewById<TextView>(Resource.Id.DeviceLabel));
        public RecyclerView RecyclerView => _recyclerView ?? (_recyclerView = FindViewById<RecyclerView>(Resource.Id.RecyclerView));

        #endregion

        class ServiceViewHolder : BindingViewHolderBase<DeviceServiceViewModel>
        {
            private readonly View _view;

            public ServiceViewHolder(View view) : base(view)
            {
                _view = view;
            }

            protected override void SetBindings()
            {

            }

            private TextView _serviceNameLabel;
            private TextView _serviceUuidLabel;
            private FrameLayout _clickSurface;

            public TextView ServiceNameLabel => _serviceNameLabel ?? (_serviceNameLabel = _view.FindViewById<TextView>(Resource.Id.ServiceNameLabel));
            public TextView ServiceUuidLabel => _serviceUuidLabel ?? (_serviceUuidLabel = _view.FindViewById<TextView>(Resource.Id.ServiceUuidLabel));
            public FrameLayout ClickSurface => _clickSurface ?? (_clickSurface = _view.FindViewById<FrameLayout>(Resource.Id.ClickSurface));

        }

        class CharacteristicViewHolder : BindingViewHolderBase<DeviceCharacteristicViewModel>
        {
            private readonly View _view;

            public CharacteristicViewHolder(View view) : base(view)
            {
                _view = view;
            }

            protected override void SetBindings()
            {

            }

            private TextView _characteristicNameLabel;
            private TextView _characteristicUuidLabel;
            private ImageView _iconCanRead;
            private ImageView _iconCanWrite;
            private ImageView _iconCanNotify;
            private FrameLayout _clickSurface;

            public TextView CharacteristicNameLabel => _characteristicNameLabel ?? (_characteristicNameLabel = _view.FindViewById<TextView>(Resource.Id.CharacteristicNameLabel));
            public TextView CharacteristicUuidLabel => _characteristicUuidLabel ?? (_characteristicUuidLabel = _view.FindViewById<TextView>(Resource.Id.CharacteristicUuidLabel));
            public ImageView IconCanRead => _iconCanRead ?? (_iconCanRead = _view.FindViewById<ImageView>(Resource.Id.IconCanRead));
            public ImageView IconCanWrite => _iconCanWrite ?? (_iconCanWrite = _view.FindViewById<ImageView>(Resource.Id.IconCanWrite));
            public ImageView IconCanNotify => _iconCanNotify ?? (_iconCanNotify = _view.FindViewById<ImageView>(Resource.Id.IconCanNotify));
            public FrameLayout ClickSurface => _clickSurface ?? (_clickSurface = _view.FindViewById<FrameLayout>(Resource.Id.ClickSurface));
        }


    }
}