using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Bussiness.Layer.Model
{
    public class Driver : NotifyPropertyChanged
    {
        public int Id { get; set; }
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnNotifyPropertyChanged("Name");
            }
        }
        public IList<Transport> Transports { get; set; }

        public Driver()
        {
            Transports = new ObservableCollection<Transport>();
        }

        public void AddTransport(Transport transport)
        {
            if (transport != null)
                Transports.Add(transport);
        }
    }
}
