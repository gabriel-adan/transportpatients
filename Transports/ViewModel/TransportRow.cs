using Bussiness.Layer.Model;
using System.Collections.ObjectModel;
using System.Linq;

namespace Transports.ViewModel
{
    public class TransportRow : ViewModelBase
    {
        public TransportRow() : base()
        {
            Drivers = new ObservableCollection<Driver>();
            EntryIndex = -1;
            ExitIndex = -1;
        }

        public ObservableCollection<Driver> Drivers { get; set; }
        
        public Transport Transport { get; set; }

        private bool _isCanceled;
        public bool IsCanceled
        {
            get { return !_isCanceled; }
            set
            {
                _isCanceled = value;
                NotifyPropertyChanged("IsCanceled");
                Icon = value ? "/Transports;component/Images/cancel.ico" : "/Transports;component/Images/ok.ico";
                if (value)
                {
                    RowState = ERowStates.CANCELED;
                }
                else
                {
                    ChangeRowStateDriverSelected();
                }
                if (Transport != null)
                {
                    Transport.IsCanceled = value;
                    if (value)
                    {
                        Transport.EntryDriver = null;
                        Transport.ExitDriver = null;
                        EntryIndex = -1;
                        ExitIndex = -1;
                        RowState = ERowStates.CANCELED;
                    }
                }
            }
        }

        private Driver _entryDriver;
        public Driver EntryDriver
        {
            get { return _entryDriver; }
            set
            {
                _entryDriver = value;
                if (value != null)
                {
                    var driver = Drivers.Where(d => d.Id.Equals(value.Id)).FirstOrDefault();
                    EntryIndex = Drivers.IndexOf(driver);
                }
                NotifyPropertyChanged("EntryDriver");
                ChangeRowStateDriverSelected();
                if (Transport != null)
                {
                    Transport.EntryDriver = value;
                }
            }
        }
        private Driver _exitDriver;
        public Driver ExitDriver
        {
            get { return _exitDriver; }
            set
            {
                _exitDriver = value;
                if (value != null)
                {
                    var driver = Drivers.Where(d => d.Id.Equals(value.Id)).FirstOrDefault();
                    ExitIndex = Drivers.IndexOf(driver);
                }
                NotifyPropertyChanged("ExitDriver");
                ChangeRowStateDriverSelected();
                if (Transport != null)
                {
                    Transport.ExitDriver = value;
                }
            }
        }

        private int _entryIndex;
        public int EntryIndex
        {
            get { return _entryIndex; }
            set
            {
                _entryIndex = value;
                NotifyPropertyChanged("EntryIndex");
            }
        }

        private int _exitIndex;
        public int ExitIndex
        {
            get { return _exitIndex; }
            set
            {
                _exitIndex = value;
                NotifyPropertyChanged("ExitIndex");
            }
        }

        private ERowStates _rowState;
        public ERowStates RowState
        {
            get { return _rowState; }
            set
            {
                _rowState = value;
                NotifyPropertyChanged("RowState");
            }
        }

        private string _icon;
        public string Icon
        {
            get { return _icon; }
            set
            {
                _icon = value;
                NotifyPropertyChanged("Icon");
            }
        }

        void ChangeRowStateDriverSelected()
        {
            if (_entryDriver == null && _exitDriver != null)
            {
                RowState = ERowStates.INCOMPLETED;
                return;
            }

            if (_entryDriver != null && _exitDriver == null)
            {
                RowState = ERowStates.INCOMPLETED;
                return;
            }

            if (!_isCanceled && (_entryDriver != null && _exitDriver != null))
            {
                RowState = ERowStates.COMPLETED;
                return;
            }

            if (!_isCanceled && (_entryDriver == null && _exitDriver == null))
            {
                RowState = ERowStates.INITIAL;
            }
        }
    }
}
