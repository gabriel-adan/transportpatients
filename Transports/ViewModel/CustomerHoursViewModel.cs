using Bussiness.Layer.Model;
using Data.Layer.Repository;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Transports.Commands;

namespace Transports.ViewModel
{
    public class CustomerHoursViewModel : ViewModelBase
    {
        public ICommand AddCustomerCommand { get; set; }
        public ICommand AddHourCommand { get; set; }
        public ICommand UpdateHourCommand { get; set; }
        public ICommand DeleteHourCommand { get; set; }
        public ICommand DeleteCustomerCommand { get; set; }
        public ICommand OpenUpdateCommand { get; set; }
        public ICommand UpdateCustomerCommand { get; set; }
        public ICommand CancelUpdateCommand { get; set; }
        public CustomerHoursViewModel() : base()
        {
            Customers = new ObservableCollection<Customer>(Context.GetCustomerList());
            Hour = new Hour();
            IndexDay = -1;
            StateVisibility = Visibility.Hidden;
            IsEditing = Visibility.Visible;
            AddCustomerCommand = new RelayCommand(c => {
                int customerId = Context.AddCustomer(_name);
                if (customerId > 0)
                {
                    Customers.Add(new Customer() { Id = customerId, Name = _name });
                    Name = string.Empty;
                }
            });
            AddHourCommand = new RelayCommand(c => {
                int v = Context.AddHour(Hour, CustomerSelected.Id);
                if (v > 0)
                {
                    CustomerSelected.Hours.Add(_hour);
                    Hour = new Hour();
                }
            }, c => (IndexDay > -1 ) && CustomerSelected != null);
            UpdateHourCommand = new RelayCommand(c => {
                MessageBoxResult result = MessageBox.Show("¿Está seguro que desea ACTUALIZAR el horario?", "Atención", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (result == MessageBoxResult.OK)
                {
                    if (Context.UpdateHour(_oldHour, SelectedHour, CustomerSelected.Id) > 0)
                    {
                        MessageBox.Show("Horario modificado exitosamente!", "Atención", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("No se pudo modificar el horario", "Atención", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    SelectedHour.DayOfWeek = _oldHour.DayOfWeek;
                    SelectedHour.Place = _oldHour.Place;
                    SelectedHour.EntryTime = _oldHour.EntryTime;
                    SelectedHour.ExitTime = _oldHour.ExitTime;
                }
            });
            DeleteHourCommand = new RelayCommand(c => {
                MessageBoxResult result = MessageBox.Show("¿Está seguro que desea ELIMINAR el horario?", "Atención", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (result == MessageBoxResult.OK)
                {
                    if (Context.DeleteHour(SelectedHour, CustomerSelected.Id))
                    {
                        CustomerSelected.Hours.Remove(SelectedHour);
                        MessageBox.Show("Horario eliminado exitosamente!", "Atención", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("No se pudo eliminar el horario", "Atención", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }, c => SelectedHour != null);
            DeleteCustomerCommand = new RelayCommand(c => {
                MessageBoxResult result = MessageBox.Show("¿Está seguro que desea ELIMINAR el cliente?", "Atención", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (result == MessageBoxResult.OK)
                {
                    if (Context.DeleteCustomer(CustomerSelected))
                    {
                        Customers.Remove(CustomerSelected);
                        MessageBox.Show("Cliente eliminado exitosamente!", "Atención", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("No se pudo eliminar el Cliente", "Atención", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            });
            OpenUpdateCommand = new RelayCommand(c => {
                StateVisibility = Visibility.Visible;
                IsEditing = Visibility.Hidden;
                EditName = CustomerSelected.Name;
            });
            UpdateCustomerCommand = new RelayCommand(c => {
                MessageBoxResult result = MessageBox.Show("¿Está seguro que desea MODIFICAR el nombre del cliente?", "Atención", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (result == MessageBoxResult.OK)
                {
                    if (Context.UpdateCustomer(CustomerSelected))
                    {
                        StateVisibility = Visibility.Hidden;
                        IsEditing = Visibility.Visible;
                        MessageBox.Show("Cliente modificado exitosamente!", "Atención", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        CustomerSelected.Name = EditName;
                        MessageBox.Show("No se pudo modificar el Cliente", "Atención", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            });
            CancelUpdateCommand = new RelayCommand(c => {
                StateVisibility = Visibility.Hidden;
                IsEditing = Visibility.Visible;
                CustomerSelected.Name = EditName;
            });
        }

        public ObservableCollection<Customer> Customers { get; set; }

        private Customer _customerSelected;
        public Customer CustomerSelected
        {
            get { return _customerSelected; }
            set
            {
                _customerSelected = value;
                NotifyPropertyChanged("CustomerSelected");
            }
        }

        private Hour _hour;
        public Hour Hour
        {
            get { return _hour; }
            set
            {
                _hour = value;
                NotifyPropertyChanged("Hour");
            }
        }

        private Hour _selectedHour;
        public Hour SelectedHour
        {
            get { return _selectedHour; }
            set
            {
                _selectedHour = value;
                if (value != null)
                {
                    _oldHour = new Hour() { Place = value.Place, EntryTime = value.EntryTime, ExitTime = value.ExitTime, DayOfWeek = value.DayOfWeek };
                }
                NotifyPropertyChanged("SelectedHour");
            }
        }

        private Hour _oldHour;
        private string _editName;
        public string EditName
        {
            get { return _editName; }
            set
            {
                _editName = value;
                NotifyPropertyChanged("EditName");
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }

        private int _indexDay;
        public int IndexDay
        {
            get { return _indexDay; }
            set
            {
                _indexDay = value;
                NotifyPropertyChanged("IndexDay");
            }
        }

        private Visibility _stateVisibility;
        public Visibility StateVisibility
        {
            get { return _stateVisibility; }
            set
            {
                _stateVisibility = value;
                NotifyPropertyChanged("StateVisibility");
            }
        }

        private Visibility _isEditing;
        public Visibility IsEditing
        {
            get { return _isEditing; }
            set
            {
                _isEditing = value;
                NotifyPropertyChanged("IsEditing");
            }
        }
    }
}
