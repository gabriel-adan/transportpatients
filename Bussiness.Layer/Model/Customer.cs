using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Bussiness.Layer.Model
{
    public class Customer : NotifyPropertyChanged
    {
        public int Id { get; set; }
        private string _name;
        public string Name {
            get { return _name; }
            set
            {
                _name = value;
                OnNotifyPropertyChanged("Name");
            }
        }

        public Hour Hour { get; set; }

        public ICollection<Hour> Hours { get; set; }

        public Customer() : base()
        {
            Hours = new ObservableCollection<Hour>();
        }
    }
}
