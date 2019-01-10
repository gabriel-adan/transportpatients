namespace Bussiness.Layer.Model
{
    public class Transport : NotifyPropertyChanged
    {
        public bool IsCanceled { get; set; }

        public Customer Customer { get; set; }
        private Driver _entryDriver;
        public Driver EntryDriver {
            get { return _entryDriver; }
            set
            {
                _entryDriver = value;
                if (value != null)
                {
                    EntryTime = Customer.Hour.EntryTime;
                }
                OnNotifyPropertyChanged("EntryDriver");
            }
        }
        private Driver _exitDriver;
        public Driver ExitDriver {
            get { return _exitDriver; }
            set
            {
                _exitDriver = value;
                if (value != null)
                {
                    ExitTime = Customer.Hour.ExitTime;
                }
                OnNotifyPropertyChanged("ExitDriver");
            }
        }

        private string _entryTime;
        public string EntryTime
        {
            get { return _entryTime; }
            set
            {
                _entryTime = value;
                OnNotifyPropertyChanged("EntryTime");
            }
        }

        private string _exitTime;
        public string ExitTime
        {
            get { return _exitTime; }
            set
            {
                _exitTime = value;
                OnNotifyPropertyChanged("ExitTime");
            }
        }
    }
}
