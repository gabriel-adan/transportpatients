namespace Bussiness.Layer.Model
{
    public class Hour : NotifyPropertyChanged
    {
        private string _place;
        public string Place {
            get { return _place; }
            set
            {
                _place = value;
                OnNotifyPropertyChanged("Place");
            }
        }
        private string _entryTime;
        public string EntryTime {
            get { return _entryTime; }
            set
            {
                _entryTime = value;
                OnNotifyPropertyChanged("EntryTime");
            }
        }
        private string _exitTime;
        public string ExitTime {
            get { return _exitTime; }
            set
            {
                _exitTime = value;
                OnNotifyPropertyChanged("ExitTime");
            }
        }
        private DaysOfWeek _dayOfWeek;
        public DaysOfWeek DayOfWeek {
            get { return _dayOfWeek; }
            set
            {
                _dayOfWeek = value;
                OnNotifyPropertyChanged("DayOfWeek");
            }
        }
    }
}
