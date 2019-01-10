using Bussiness.Layer.Model;
using Data.Layer.Repository;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using Transports.Commands;

namespace Transports.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public ICommand DeleteTransportCommand { get; set; }
        public ICommand CancelTransportCommand { get; set; }
        public ObservableCollection<TransportRow> TransportRows { get; set; }
        public ObservableCollection<Driver> DriversTransports { get; set; }

        public MainViewModel(string connectionString) : base()
        {
            TransportRows = new ObservableCollection<TransportRow>();
            DriversTransports = new ObservableCollection<Driver>();
            _drivers = new ObservableCollection<Driver>();

            DeleteTransportCommand = new RelayCommand(c => {
                Driver driver = c as Driver;
                if (driver != null)
                {
                    foreach (TransportRow row in TransportRows)
                    {
                        Customer customer = row.Transport.Customer;
                        Transport transport = _transportSelected;
                        if (_transportSelected != null && (customer.Id.Equals(transport.Customer.Id) && customer.Hour.EntryTime.Equals(transport.Customer.Hour.EntryTime) && customer.Hour.ExitTime.Equals(transport.Customer.Hour.ExitTime)))
                        {
                            if (_transportSelected.EntryDriver != null && _transportSelected.ExitDriver == null)
                            {
                                row.EntryDriver = null;
                            }
                            if (_transportSelected.ExitDriver != null && _transportSelected.EntryDriver == null)
                            {
                                row.ExitDriver = null;
                            }
                            if (_transportSelected.EntryDriver != null && _transportSelected.ExitDriver != null)
                            {
                                if (_transportSelected.EntryDriver.Equals(driver))
                                {
                                    row.EntryDriver = null;
                                }
                                if (_transportSelected.ExitDriver.Equals(driver))
                                {
                                    row.ExitDriver = null;
                                }
                            }
                            if (Context.UpdateTransport(row.Transport))
                            {
                                driver.Transports.Remove(_transportSelected);
                            }
                            if (driver.Transports.Count == 0)
                                DriversTransports.Remove(driver);
                            break;
                        }
                    }
                }
            });
            CancelTransportCommand = new RelayCommand(c => {
                TransportRow row = c as TransportRow;
                if (row != null)
                {
                    Driver entry = row.Transport.EntryDriver;
                    Driver exit = row.Transport.ExitDriver;
                    row.IsCanceled = row.IsCanceled ? true : false;
                    if (!Context.UpdateTransport(row.Transport))
                    {
                        row.IsCanceled = false;
                    }
                    else
                    {
                        if (entry != null && exit != null)
                        {
                            if (entry.Id.Equals(exit.Id))
                            {
                                Driver driver = DriversTransports.Where(d => d.Id.Equals(entry.Id)).FirstOrDefault();
                                DeleteTransport(driver, row.Transport);
                                DeleteTransport(driver, row.Transport);
                            }
                            else
                            {
                                entry = DriversTransports.Where(d => d.Id.Equals(entry.Id)).FirstOrDefault();
                                DeleteTransport(entry, row.Transport);
                                exit = DriversTransports.Where(d => d.Id.Equals(exit.Id)).FirstOrDefault();
                                DeleteTransport(exit, row.Transport);
                            }
                        }
                        else
                        {
                            if (entry != null)
                            {
                                entry = DriversTransports.Where(d => d.Id.Equals(entry.Id)).FirstOrDefault();
                                DeleteTransport(entry, row.Transport);
                            }
                            if (exit != null)
                            {
                                exit = DriversTransports.Where(d => d.Id.Equals(exit.Id)).FirstOrDefault();
                                DeleteTransport(exit, row.Transport);
                            }
                        }
                    }
                }
            });

            Transport = new Transport() { Customer = new Customer() { Hour = new Hour() { DayOfWeek = DayOfWeek } }, IsCanceled = false };
            if (Context.OpenConnection(connectionString))
            {
                switch ((int)System.DateTime.Now.DayOfWeek)
                {
                    case 1:
                        DayOfWeek = DaysOfWeek.LUNES;
                        break;
                    case 2:
                        DayOfWeek = DaysOfWeek.MARTES;
                        break;
                    case 3:
                        DayOfWeek = DaysOfWeek.MIERCOLES;
                        break;
                    case 4:
                        DayOfWeek = DaysOfWeek.JUEVES;
                        break;
                    case 5:
                        DayOfWeek = DaysOfWeek.VIERNES;
                        break;
                }
            }
        }

        private ObservableCollection<Driver> _drivers;

        private DaysOfWeek _dayOfWeek;
        public DaysOfWeek DayOfWeek
        {
            get { return _dayOfWeek; }
            set
            {
                _dayOfWeek = value;
                NotifyPropertyChanged("DayOfWeek");
                _drivers.Clear();
                TransportRows.Clear();
                DriversTransports.Clear();
                _drivers = new ObservableCollection<Driver>(Context.DriversList(value));
                var transports = Context.GetCustomersTransportsByDayOfWeek(value);
                foreach (Transport transport in transports)
                {
                    TransportRows.Add(new TransportRow() { Transport = transport, Drivers = _drivers, EntryDriver = (!transport.IsCanceled) ? transport.EntryDriver : null, ExitDriver = (!transport.IsCanceled) ? transport.ExitDriver : null, IsCanceled = transport.IsCanceled });
                }
                foreach (Driver driver in _drivers)
                {
                    if (driver.Transports.Count > 0)
                        DriversTransports.Add(driver);
                }
            }
        }

        private Transport _transportSelected;
        public Transport TransportSelected
        {
            get { return _transportSelected; }
            set
            {
                _transportSelected = value;
                NotifyPropertyChanged("TransportSelected");
            }
        }

        private TransportRow _transportRowSelected;
        public TransportRow TransportRowSelected
        {
            get { return _transportRowSelected; }
            set
            {
                _transportRowSelected = value;
                NotifyPropertyChanged("TransportRowSelected");
            }
        }

        private Transport _transport;
        public Transport Transport
        {
            get { return _transport; }
            set
            {
                _transport = value;
                NotifyPropertyChanged("Transport");
            }
        }

        private bool _isDriverDeleting = false;

        public void AddEntryTransport(Driver driver, Customer customer)
        {
            if (driver != null && customer != null && !_isDriverDeleting && TransportRowSelected != null && TransportRowSelected.Transport != null)
            {
                Transport transport = new Transport() { Customer = customer, EntryDriver = TransportRowSelected.EntryDriver, ExitDriver = TransportRowSelected.ExitDriver, IsCanceled = !TransportRowSelected.IsCanceled };

                if (Context.UpdateTransport(transport))
                {
                    if (DriversTransports.Contains(driver))
                    {
                        Transport tr = driver.Transports.Where(t => (t.Customer.Hour.EntryTime.Equals(customer.Hour.EntryTime) && t.Customer.Hour.ExitTime.Equals(customer.Hour.ExitTime))).FirstOrDefault();
                        if (tr != null)
                        {
                            if (tr.EntryDriver == null)
                            {
                                tr.EntryDriver = driver;
                                foreach(Driver dr in DriversTransports.Where(d => (!d.Id.Equals(driver.Id))).ToList())
                                {
                                    Transport td = dr.Transports.Where(t => (t.Customer.Hour.EntryTime.Equals(customer.Hour.EntryTime) && t.Customer.Hour.ExitTime.Equals(customer.Hour.ExitTime))).FirstOrDefault();
                                    if (td != null)
                                    {
                                        if (td.ExitDriver == null)
                                        {
                                            dr.Transports.Remove(td);
                                            if (dr.Transports.Count == 0)
                                            {
                                                DriversTransports.Remove(dr);
                                            }
                                        }
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                Driver dr = DriversTransports.Where(d => d.Id.Equals(tr.EntryDriver.Id)).FirstOrDefault();
                                if (tr.ExitDriver != null)
                                {
                                    if (!tr.EntryDriver.Id.Equals(tr.ExitDriver.Id))
                                    {
                                        Transport td = dr.Transports.Where(t => (t.Customer.Hour.EntryTime.Equals(customer.Hour.EntryTime) && t.Customer.Hour.ExitTime.Equals(customer.Hour.ExitTime))).FirstOrDefault();
                                        if (td != null)
                                        {
                                            td.EntryDriver = null;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            transport.ExitDriver = null;
                            driver.AddTransport(transport);
                            foreach (Driver dr in DriversTransports.Where(d => (!d.Id.Equals(driver.Id))).ToList())
                            {
                                Transport td = dr.Transports.Where(t => (t.Customer.Hour.EntryTime.Equals(customer.Hour.EntryTime) && t.Customer.Hour.ExitTime.Equals(customer.Hour.ExitTime))).FirstOrDefault();
                                if (td != null)
                                {
                                    if (td.ExitDriver == null)
                                    {
                                        dr.Transports.Remove(td);
                                        if (dr.Transports.Count == 0)
                                        {
                                            DriversTransports.Remove(dr);
                                        }
                                    }
                                    else
                                    {
                                        td.EntryDriver = null;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (Driver dr in DriversTransports)
                        {
                            transport = dr.Transports.Where(t => (t.Customer.Id.Equals(customer.Id) && t.Customer.Hour.EntryTime.Equals(customer.Hour.EntryTime))).FirstOrDefault();
                            if (transport != null)
                            {
                                if (transport.ExitDriver == null && transport.EntryDriver != null)
                                {
                                    dr.Transports.Remove(transport);
                                    if (dr.Transports.Count == 0)
                                    {
                                        DriversTransports.Remove(dr);
                                        break;
                                    }
                                }
                                else
                                {
                                    transport.EntryDriver = null;
                                }
                            }
                        }

                        transport = new Transport() { Customer = customer, EntryDriver = driver };
                        driver.AddTransport(transport);
                        DriversTransports.Add(driver);
                    }
                }
            }
            _isDriverDeleting = false;
        }

        public void AddExitTransport(Driver driver, Customer customer)
        {
            if (driver != null && customer != null && !_isDriverDeleting && TransportRowSelected != null && TransportRowSelected.Transport != null)
            {
                Transport transport = new Transport() { Customer = customer, EntryDriver = TransportRowSelected.EntryDriver, ExitDriver = TransportRowSelected.ExitDriver, IsCanceled = !TransportRowSelected.IsCanceled };

                if (Context.UpdateTransport(transport))
                {
                    if (DriversTransports.Contains(driver))
                    {
                        Transport tr = driver.Transports.Where(t => (t.Customer.Hour.EntryTime.Equals(customer.Hour.EntryTime) && t.Customer.Hour.ExitTime.Equals(customer.Hour.ExitTime))).FirstOrDefault();
                        if (tr != null)
                        {
                            if (tr.ExitDriver == null)
                            {
                                tr.ExitDriver = driver;
                                foreach (Driver dr in DriversTransports.Where(d => (!d.Id.Equals(driver.Id))).ToList())
                                {
                                    Transport td = dr.Transports.Where(t => (t.Customer.Hour.EntryTime.Equals(customer.Hour.EntryTime) && t.Customer.Hour.ExitTime.Equals(customer.Hour.ExitTime))).FirstOrDefault();
                                    if (td != null)
                                    {
                                        if (td.EntryDriver == null)
                                        {
                                            dr.Transports.Remove(td);
                                            if (dr.Transports.Count == 0)
                                            {
                                                DriversTransports.Remove(dr);
                                            }
                                        }
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                Driver dr = DriversTransports.Where(d => d.Id.Equals(tr.EntryDriver.Id)).FirstOrDefault();
                                if (tr.EntryDriver != null)
                                {
                                    if (!tr.ExitDriver.Id.Equals(tr.EntryDriver.Id))
                                    {
                                        Transport td = dr.Transports.Where(t => (t.Customer.Hour.EntryTime.Equals(customer.Hour.EntryTime) && t.Customer.Hour.ExitTime.Equals(customer.Hour.ExitTime))).FirstOrDefault();
                                        if (td != null)
                                        {
                                            td.ExitDriver = null;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            transport.EntryDriver = null;
                            driver.AddTransport(transport);
                            foreach (Driver dr in DriversTransports.Where(d => (!d.Id.Equals(driver.Id))).ToList())
                            {
                                Transport td = dr.Transports.Where(t => (t.Customer.Hour.EntryTime.Equals(customer.Hour.EntryTime) && t.Customer.Hour.ExitTime.Equals(customer.Hour.ExitTime))).FirstOrDefault();
                                if (td != null)
                                {
                                    if (td.EntryDriver == null)
                                    {
                                        dr.Transports.Remove(td);
                                        if (dr.Transports.Count == 0)
                                        {
                                            DriversTransports.Remove(dr);
                                        }
                                    }
                                    else
                                    {
                                        td.ExitDriver = null;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (Driver dr in DriversTransports)
                        {
                            transport = dr.Transports.Where(t => (t.Customer.Id.Equals(customer.Id) && t.Customer.Hour.EntryTime.Equals(customer.Hour.EntryTime))).FirstOrDefault();
                            if (transport != null)
                            {
                                if (transport.EntryDriver == null && transport.ExitDriver != null)
                                {
                                    dr.Transports.Remove(transport);
                                    if (dr.Transports.Count == 0)
                                    {
                                        DriversTransports.Remove(dr);
                                        break;
                                    }
                                }
                                else
                                {
                                    transport.ExitDriver = null;
                                }
                            }
                        }

                        transport = new Transport() { Customer = customer, ExitDriver = driver };
                        driver.AddTransport(transport);
                        DriversTransports.Add(driver);
                    }
                }
            }
            _isDriverDeleting = false;
        }

        void DeleteTransport(Driver driver, Transport transport)
        {
            if (driver != null && transport != null)
            {
                driver = DriversTransports.Where(d => d.Id.Equals(driver.Id)).FirstOrDefault();
                if (driver != null)
                {
                    transport = driver.Transports.Where(t => (t.Customer.Id.Equals(transport.Customer.Id) && t.Customer.Hour.EntryTime.Equals(transport.Customer.Hour.EntryTime) && t.Customer.Hour.ExitTime.Equals(transport.Customer.Hour.ExitTime))).FirstOrDefault();
                    driver.Transports.Remove(transport);
                    if (driver.Transports.Count == 0)
                        DriversTransports.Remove(driver);
                }
            }
        }
    }
}