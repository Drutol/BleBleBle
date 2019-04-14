using BleBleBle.Domain.Models;
using BleBleBle.Shared.Interfaces;

namespace BleBleBle.Shared.ViewModels.Items.Messages
{
    public class ReceivedCharacteristicMessageViewModel : CharacteristicMessageViewModelBase, IDeviceCharacteristicChatListItem
    {
        public ReceivedCharacteristicMessage Message { get; }

        public ReceivedCharacteristicMessageViewModel(ReceivedCharacteristicMessage message)
        {
            Message = message;
        }
    }
}
