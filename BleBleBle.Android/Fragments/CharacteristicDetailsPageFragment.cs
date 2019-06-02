using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using AoLibs.Utilities.Android.Views;
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

        private static bool _readCharacteristic;

        protected override void InitBindings()
        {
            Bindings.Add(
                this.SetBinding(() => ViewModel.AreNotificationsEnabled,
                    () => EnableNotificationsCheckbox.Checked, BindingMode.TwoWay));

            Bindings.Add(
                this.SetBinding(() => ViewModel.UseHex,
                    () => RepresentationSwitch.Checked, BindingMode.TwoWay));

            Bindings.Add(this.SetBinding(() => ViewModel.Characteristic).WhenSourceChanges(() =>
            {
                if (ViewModel.Characteristic == null)
                    return;

                WriteInput.Visibility = ViewModel.Characteristic.CanWrite ? ViewStates.Visible : ViewStates.Gone;
                EnableNotificationsCheckbox.Visibility = ViewModel.Characteristic.CanUpdate && !_readCharacteristic ? ViewStates.Visible : ViewStates.Gone;
                PullToReadLabel.Visibility = ViewModel.Characteristic.CanRead ? ViewStates.Visible : ViewStates.Gone;
                RefreshLayout.Enabled = ViewModel.Characteristic.CanRead;
                RefreshLayout.ScrollingView = ChatRecyclerView;
                RefreshLayout.Refresh += RefreshLayoutOnRefresh;
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
                                    LayoutInflater.Inflate(Resource.Layout.item_characteristic_message_received, null),
                                SpecializedDataTemplate = ReceivedDataTemplate
                            }
                        }
                    },
                    ViewModel.ChatMessages) {StretchContentHorizonatally = true});
            ChatRecyclerView.SetLayoutManager(new LinearLayoutManager(Activity, LinearLayoutManager.Vertical, true)
            {
                StackFromEnd = true,
                ReverseLayout = true
            });

            SendButton.SetOnClickListener(new OnClickListener(view =>
            {
                ViewModel.SendMessageCommand.Execute(CommandInput.Text);
            }));
        }

        private async void RefreshLayoutOnRefresh(object sender, EventArgs e)
        {
            _readCharacteristic = true;
            PullToReadLabel.Visibility = ViewStates.Gone;
            ViewModel.ReadOnceCommand.Execute(null);
            await Task.Delay(300);
            RefreshLayout.Refreshing = false;
        }

        public override void NavigatedTo()
        {
            ViewModel.NavigatedTo(NavigationArguments as DeviceCharacteristicsDetailsNavArgs);
            RefreshLayoutOnRefresh(this, EventArgs.Empty);
        }

        public override void NavigatedFrom()
        {
            ViewModel.NavigatedFrom();
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

        private Switch _representationSwitch;
        private TextView _representationLabel;
        private CheckBox _enableNotificationsCheckbox;
        private TextView _pullToReadLabel;
        private RecyclerView _chatRecyclerView;
        private ScrollableSwipeToRefreshLayout _refreshLayout;
        private TextInputEditText _commandInput;
        private ImageButton _sendButton;
        private LinearLayout _writeInput;

        public Switch RepresentationSwitch => _representationSwitch ?? (_representationSwitch = FindViewById<Switch>(Resource.Id.RepresentationSwitch));
        public TextView RepresentationLabel => _representationLabel ?? (_representationLabel = FindViewById<TextView>(Resource.Id.RepresentationLabel));
        public CheckBox EnableNotificationsCheckbox => _enableNotificationsCheckbox ?? (_enableNotificationsCheckbox = FindViewById<CheckBox>(Resource.Id.EnableNotificationsCheckbox));
        public TextView PullToReadLabel => _pullToReadLabel ?? (_pullToReadLabel = FindViewById<TextView>(Resource.Id.PullToReadLabel));
        public RecyclerView ChatRecyclerView => _chatRecyclerView ?? (_chatRecyclerView = FindViewById<RecyclerView>(Resource.Id.ChatRecyclerView));
        public ScrollableSwipeToRefreshLayout RefreshLayout => _refreshLayout ?? (_refreshLayout = FindViewById<ScrollableSwipeToRefreshLayout>(Resource.Id.RefreshLayout));
        public TextInputEditText CommandInput => _commandInput ?? (_commandInput = FindViewById<TextInputEditText>(Resource.Id.CommandInput));
        public ImageButton SendButton => _sendButton ?? (_sendButton = FindViewById<ImageButton>(Resource.Id.SendButton));
        public LinearLayout WriteInput => _writeInput ?? (_writeInput = FindViewById<LinearLayout>(Resource.Id.WriteInput));

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