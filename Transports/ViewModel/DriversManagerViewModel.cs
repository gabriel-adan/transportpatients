using Bussiness.Layer.Model;
using Data.Layer.Repository;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Transports.Commands;

namespace Transports.ViewModel
{
    public class DriversManagerViewModel : ViewModelBase
    {
        public ICommand AddDriverCommand { get; set; }
        public ICommand UpdateDriverCommand { get; set; }
        public ICommand DeleteDriverCommand { get; set; }
        public ICommand ModifyCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public DriversManagerViewModel() : base()
        {
            Drivers = new ObservableCollection<Driver>(Context.DriversList());
            UpdateVisibility = Visibility.Hidden;
            AddDriverCommand = new RelayCommand(c =>
            {
                int id = Context.AddDriver(Name);
                if (id > 0)
                {
                    Drivers.Add(new Driver() { Id = id, Name = Name });
                    Name = string.Empty;
                }
            }, c => !string.IsNullOrEmpty(Name));
            UpdateDriverCommand = new RelayCommand(c => {
                MessageBoxResult result = MessageBox.Show("¿Está seguro que desea ACTUALIZAR el nombre del chofer?", "Atención", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (result == MessageBoxResult.OK)
                {
                    if (Context.UpdateDriver(DriverSelected))
                    {
                        UpdateVisibility = Visibility.Hidden;
                        MessageBox.Show("Chofer modificado exitosamente!", "Atención", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        UpdateVisibility = Visibility.Hidden;
                        MessageBox.Show("No se pudo modificar el nombre del chofer", "Atención", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    DriverSelected.Name = EditName;
                    UpdateVisibility = Visibility.Hidden;
                }
            }, c => (DriverSelected != null && !string.IsNullOrEmpty(DriverSelected.Name)));
            DeleteDriverCommand = new RelayCommand(c => {
                MessageBoxResult result = MessageBox.Show("¿Está seguro que desea ELIMINAR el chofer?", "Atención", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (result == MessageBoxResult.OK)
                {
                    if (Context.DeleteDriver(DriverSelected))
                    {
                        Drivers.Remove(DriverSelected);
                        MessageBox.Show("Chofer eliminado exitosamente!", "Atención", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("No se pudo eliminar el chofer", "Atención", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            });
            ModifyCommand = new RelayCommand(c => {
                UpdateVisibility = Visibility.Visible;
                EditName = DriverSelected.Name;
            });
            CancelCommand = new RelayCommand(c => {
                UpdateVisibility = Visibility.Hidden;
                DriverSelected.Name = EditName;
            });
        }

        public ObservableCollection<Driver> Drivers { get; set; }

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

        private Driver _driverSelected;
        public Driver DriverSelected
        {
            get { return _driverSelected; }
            set
            {
                _driverSelected = value;
                NotifyPropertyChanged("DriverSelected");
            }
        }

        private Visibility _updateVisibility;
        public Visibility UpdateVisibility
        {
            get { return _updateVisibility; }
            set
            {
                _updateVisibility = value;
                NotifyPropertyChanged("UpdateVisibility");
            }
        }
    }
}
