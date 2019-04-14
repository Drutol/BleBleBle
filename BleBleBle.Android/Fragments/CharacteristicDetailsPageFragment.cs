﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
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
using BleBleBle.Shared.ViewModels.Items.Messages;
using GalaSoft.MvvmLight.Helpers;

namespace BleBleBle.Android.Fragments
{
    [NavigationPage(PageIndex.CharacteristicDetailsPage, NavigationPageAttribute.PageProvider.Cached)]
    public class CharacteristicDetailsPageFragment : FragmentBase<CharacteristicDetailsViewModel>
    {
        public override int LayoutResourceId { get; } = Resource.Layout.characteristics_details_page;

        protected override void InitBindings()
        {
            SendButton.SetOnClickListener(new OnClickListener(view =>
            {
                ViewModel.SendMessageCommand.Execute(CommandInput.Text);
            }));


            ChatRecyclerView.SetAdapter(
                new ObservableRecyclerAdapterWithMultipleViewTypes<IDeviceCharacteristicChatListItem,
                    RecyclerView.ViewHolder>(
                    new Dictionary<Type, ObservableRecyclerAdapterWithMultipleViewTypes<
                        IDeviceCharacteristicChatListItem, RecyclerView.ViewHolder>.IItemEntry>
                    {
                        {
                            typeof(SentCharacteristicMessageViewModel),
                            new ObservableRecyclerAdapterWithMultipleViewTypes<IDeviceCharacteristicChatListItem,
                                RecyclerView.ViewHolder>.SpecializedItemEntry<SentCharacteristicMessageViewModel,
                                SentMessageHolder>
                            {
                                ItemTemplate = type =>
                                    LayoutInflater.Inflate(Resource.Layout.item_characteristic_message_sent, null),
                                SpecializedDataTemplate = SentDataTemplate
                            }
                        },
                        {
                            typeof(ReceivedCharacteristicMessageViewModel),
                            new ObservableRecyclerAdapterWithMultipleViewTypes<IDeviceCharacteristicChatListItem,
                                RecyclerView.ViewHolder>.SpecializedItemEntry<ReceivedCharacteristicMessageViewModel,
                                ReceivedMessageHolder>
                            {
                                ItemTemplate = type =>
                                    LayoutInflater.Inflate(Resource.Layout.item_characteristic_message_sent, null),
                                SpecializedDataTemplate = ReceivedDataTemplate
                            }
                        }
                    },
                    ViewModel.ChatMessages) {StretchContentHorizonatally = true});
            ChatRecyclerView.SetLayoutManager(new LinearLayoutManager(Activity, LinearLayoutManager.Vertical, true));
        }

        public override void NavigatedTo()
        {
            ViewModel.NavigatedTo(NavigationArguments as DeviceCharacteristicsDetailsNavArgs);
        }

        private void ReceivedDataTemplate(ReceivedCharacteristicMessageViewModel item, ReceivedMessageHolder holder, int position)
        {
            holder.MessageContent.Text = item.Message.Content;
            holder.TimeLabel.Text = item.Message.DateTime.ToString("HH:mm");
        }

        private void SentDataTemplate(SentCharacteristicMessageViewModel item, SentMessageHolder holder, int position)
        {
            holder.MessageContent.Text = item.Message.Content;
            holder.TimeLabel.Text = item.Message.DateTime.ToString("HH:mm");
        }

        #region Views

        private RecyclerView _chatRecyclerView;
        private TextInputEditText _commandInput;
        private ImageButton _sendButton;

        public RecyclerView ChatRecyclerView => _chatRecyclerView ?? (_chatRecyclerView = FindViewById<RecyclerView>(Resource.Id.ChatRecyclerView));
        public TextInputEditText CommandInput => _commandInput ?? (_commandInput = FindViewById<TextInputEditText>(Resource.Id.CommandInput));
        public ImageButton SendButton => _sendButton ?? (_sendButton = FindViewById<ImageButton>(Resource.Id.SendButton));

        #endregion

        class SentMessageHolder : BindingViewHolderBase<SentCharacteristicMessageViewModel>
        {
            private readonly View _view;

            public SentMessageHolder(View view) : base(view)
            {
                _view = view;
            }

            protected override void SetBindings()
            {
                Bindings.Add(this.SetBinding(() => ViewModel.SuccessfullySent).WhenSourceChanges(() =>
                {
                    if (ViewModel.SuccessfullySent)
                    {
                        TimeLabel.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.icon_double_tick, 0, 0, 0);
                    }
                }));
            }

            private ImageView _moreIndicator;
            private TextView _messageContent;
            private TextView _timeLabel;

            public ImageView MoreIndicator => _moreIndicator ?? (_moreIndicator = _view.FindViewById<ImageView>(Resource.Id.MoreIndicator));
            public TextView MessageContent => _messageContent ?? (_messageContent = _view.FindViewById<TextView>(Resource.Id.MessageContent));
            public TextView TimeLabel => _timeLabel ?? (_timeLabel = _view.FindViewById<TextView>(Resource.Id.TimeLabel));

        }

        class ReceivedMessageHolder : BindingViewHolderBase<ReceivedCharacteristicMessageViewModel>
        {
            private readonly View _view;

            public ReceivedMessageHolder(View view) : base(view)
            {
                _view = view;
            }

            protected override void SetBindings()
            {
                Bindings.Add(this.SetBinding(() => ViewModel.SuccessfullySent).WhenSourceChanges(() =>
                {
                    if (ViewModel.SuccessfullySent)
                    {
                        TimeLabel.SetCompoundDrawablesWithIntrinsicBounds(0, 0, Resource.Drawable.icon_double_tick, 0);
                    }
                }));
            }

            private ImageView _moreIndicator;
            private TextView _messageContent;
            private TextView _timeLabel;

            public ImageView MoreIndicator => _moreIndicator ?? (_moreIndicator = _view.FindViewById<ImageView>(Resource.Id.MoreIndicator));
            public TextView MessageContent => _messageContent ?? (_messageContent = _view.FindViewById<TextView>(Resource.Id.MessageContent));
            public TextView TimeLabel => _timeLabel ?? (_timeLabel = _view.FindViewById<TextView>(Resource.Id.TimeLabel));

        }


    }
}