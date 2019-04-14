using BleBleBle.Domain.Models;
using BleBleBle.Shared.Interfaces;

namespace BleBleBle.Shared.ViewModels.Items.Messages
{
    public class SentCharacteristicMessageViewModel : CharacteristicMessageViewModelBase, IDeviceCharacteristicChatListItem
    {
        public SentCharacteristicMessage Message { get; }

        public SentCharacteristicMessageViewModel(SentCharacteristicMessage message)
        {
            Message = message;
        }
    }
}
