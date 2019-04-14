using GalaSoft.MvvmLight;

namespace BleBleBle.Shared.ViewModels.Items.Messages
{
    public class CharacteristicMessageViewModelBase : ViewModelBase
    {
        private bool _successfullySent;

        public bool SuccessfullySent
        {
            get => _successfullySent;
            set
            {
                _successfullySent = value;
                RaisePropertyChanged();
            }
        }
    }
}
