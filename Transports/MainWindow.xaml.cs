using Bussiness.Layer.Model;
using Data.Layer.Repository;
using Logger.Layer.Log.Service;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Transports.Properties;
using Transports.ViewModel;

namespace Transports
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainViewModel mainViewModel;
        public MainWindow()
        {
            InitializeComponent();
            Height = SystemParameters.WorkArea.Height;
            Width = SystemParameters.WorkArea.Width - (SystemParameters.WorkArea.Width * .05);
            try
            {
                mainViewModel = new MainViewModel(Settings.Default.ConnectionString);
                DataContext = mainViewModel;
                if (!Context.IsConnected)
                {
                    MessageBox.Show("No se pudo abrir el archivo de Datos", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                LoggerManager.HandleException(ex);
            }
        }

        private void ComboBox_Entry_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (((ComboBox)sender).IsDropDownOpen)
                {
                    Driver driver = (sender as ComboBox).SelectedValue as Driver;
                    TransportRow transportRow = CustomersGrid.SelectedValue as TransportRow;
                    if (transportRow != null && transportRow.EntryDriver != null)
                    {
                        mainViewModel.AddEntryTransport(driver, transportRow.Transport.Customer);
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerManager.HandleException(ex);
            }
        }

        private void ComboBox_Exit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (((ComboBox)sender).IsDropDownOpen)
                {
                    Driver driver = (sender as ComboBox).SelectedValue as Driver;
                    TransportRow transportRow = CustomersGrid.SelectedValue as TransportRow;
                    if (transportRow != null && transportRow.ExitDriver != null)
                    {
                        mainViewModel.AddExitTransport(driver, transportRow.Transport.Customer);
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerManager.HandleException(ex);
            }
        }

        private void DataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            if (!e.Column.SortMemberPath.Equals("EntryTime"))
                return;
            e.Handled = true;
        }

        private void DataGrid_Initialized(object sender, EventArgs e)
        {
            try
            {
                DataGridColumn column = ((DataGrid)sender).Columns[2];
                DataGrid_Sorting(sender, new DataGridSortingEventArgs(column));
                ListCollectionView view = (ListCollectionView)CollectionViewSource.GetDefaultView(((DataGrid)sender).ItemsSource);
                view.CustomSort = new TransportSortingByEntryTime();
                column.SortDirection = System.ComponentModel.ListSortDirection.Ascending;
            }
            catch(Exception ex)
            {
                LoggerManager.HandleException(ex);
            }
        }
    }
}
