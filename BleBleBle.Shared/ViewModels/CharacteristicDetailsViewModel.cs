using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace BleBleBle.Shared.ViewModels
{
    public class CharacteristicDetailsViewModel : ViewModelBase
    {
        private readonly INavigationManager<PageIndex> _navigationManager;
        private readonly IMessageBoxProvider _messageBoxProvider;
        private ICharacteristic _characteristic;

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

        public CharacteristicDetailsViewModel(INavigationManager<PageIndex> navigationManager, IMessageBoxProvider messageBoxProvider)
        {
            _navigationManager = navigationManager;
            _messageBoxProvider = messageBoxProvider;
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
    }
}
