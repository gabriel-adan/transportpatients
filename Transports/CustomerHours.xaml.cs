using Bussiness.Layer.Model;
using Data.Layer.Repository;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Transports.ViewModel;

namespace Transports
{
    /// <summary>
    /// Lógica de interacción para CustomerHours.xaml
    /// </summary>
    public partial class CustomerHours : Window
    {
        CustomerHoursViewModel _customerHoursViewModel;
        public CustomerHours()
        {
            InitializeComponent();
            Height = SystemParameters.PrimaryScreenHeight - 50;
            _customerHoursViewModel = new CustomerHoursViewModel();
            DataContext = _customerHoursViewModel;
            listBox.ItemsSource = _customerHoursViewModel.Customers;
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listBox.ItemsSource);
            view.Filter = CustomerFilter;
        }

        private bool CustomerFilter(object item)
        {
            if (string.IsNullOrEmpty(txtSearch.Text))
                return true;
            else
                return ((item as Customer).Name.IndexOf(txtSearch.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(listBox.ItemsSource).Refresh();
        }
    }
}
