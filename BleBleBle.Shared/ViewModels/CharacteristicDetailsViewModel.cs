using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AoLibs.Adapters.Core.Interfaces;
using AoLibs.Navigation.Core.Interfaces;
using BleBleBle.Domain.Enums;
using BleBleBle.Domain.Models;
using BleBleBle.Shared.Interfaces;
using BleBleBle.Shared.NavArgs;
using BleBleBle.Shared.Statics;
using BleBleBle.Shared.Utils;
using BleBleBle.Shared.ViewModels.Items;
using BleBleBle.Shared.ViewModels.Items.Messages;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;

namespace BleBleBle.Shared.ViewModels
{
    public class CharacteristicDetailsViewModel : ViewModelBase
    {
        private readonly INavigationManager<PageIndex> _navigationManager;
        private readonly IMessageBoxProvider _messageBoxProvider;
        private readonly IDispatcherAdapter _dispatcherAdapter;
        private ICharacteristic _characteristic;
        private bool _areNotificationsEnabled;

        public ObservableCollection<IDeviceCharacteristicChatListItem> ChatMessages { get; set; } =
            new ObservableCollection<IDeviceCharacteristicChatListItem>();

        public ICharacteristic Characteristic
        {
            get => _characteristic;
            set
            {
                _characteristic = value;
                RaisePropertyChanged();
            }
        }

        public bool AreNotificationsEnabled
        {
            get => _areNotificationsEnabled;
            set
            {
                if (value != _areNotificationsEnabled)
                {
                    if (value)
                        EnableNotifications();
                    else
                        DisableNotifications();
                }
                _areNotificationsEnabled = value;
                RaisePropertyChanged();


            }
        }

        public void NavigatedFrom()
        {
            AreNotificationsEnabled = false;
        }

        private async void DisableNotifications()
        {
            _characteristic.ValueUpdated -= CharacteristicOnValueUpdated;
            await _characteristic.StopUpdatesAsync();
        }

        private async void EnableNotifications()
        {
            _characteristic.ValueUpdated += CharacteristicOnValueUpdated;
            await _characteristic.StartUpdatesAsync();
        }

        private void CharacteristicOnValueUpdated(object sender, CharacteristicUpdatedEventArgs e)
        {
            _dispatcherAdapter.Run(() =>
            {
                using (var scope = ResourceLocator.ObtainScope())
                {
                    var messageModel = scope.TypedResolve<ReceivedCharacteristicMessageViewModel>(
                        new ReceivedCharacteristicMessage
                        {
                            Content = e.Characteristic.StringValue,
                            DateTime = DateTime.Now,
                        });
                    ChatMessages.Insert(0, messageModel);
                }
            });
        }

        public CharacteristicDetailsViewModel(INavigationManager<PageIndex> navigationManager,
            IDispatcherAdapter dispatcherAdapter)
        {
            _navigationManager = navigationManager;
            _messageBoxProvider = messageBoxProvider;
            _dispatcherAdapter = dispatcherAdapter;
        }

        public void NavigatedTo(DeviceCharacteristicsDetailsNavArgs navArgs)
        {
            ChatMessages.Clear();
            Characteristic = navArgs.Characteristic;
        }
       
        public RelayCommand<string> SendMessageCommand => new RelayCommand<string>(async message =>
        {
            using (var scope = ResourceLocator.ObtainScope())
            {
                try
                {
                    var messageModel = scope.TypedResolve<SentCharacteristicMessageViewModel>(
                        new SentCharacteristicMessage
                        {
                            Content = message,
                            DateTime = DateTime.Now
                        });

                    ChatMessages.Add(messageModel);

                    var resp = await Characteristic.WriteAsync(Encoding.UTF8.GetBytes(message));

                    await Task.Delay(500);
                    messageModel.SuccessfullySent = resp;

                }
                catch (Exception e)
                {
                    if (e.Message.Contains("Characteristic does not support write"))
                        await _messageBoxProvider.ShowMessageBoxOkAsync(string.Empty, "Characteristic don't have write permission", "OK");
                }
            }
        });

        public RelayCommand ReadOnceCommand => new RelayCommand(async () =>
        {
            using (var scope = ResourceLocator.ObtainScope())
            {
                if (AreNotificationsEnabled)
                    await _characteristic.StopUpdatesAsync();

                var resp = await Characteristic.ReadAsync();

                if (AreNotificationsEnabled)
                    await _characteristic.StartUpdatesAsync();

                string message = null;
                if (resp != null && resp.Any())
                {
                    message = Encoding.UTF8.GetString(resp);
                }
                else
                {
                    message = "N/A";
                }

                var messageModel = scope.TypedResolve<ReceivedCharacteristicMessageViewModel>(new ReceivedCharacteristicMessage
                {
                    Content = message,
                    DateTime = DateTime.Now
                });
                ChatMessages.Insert(0, messageModel);
            }
        });
    }
}
