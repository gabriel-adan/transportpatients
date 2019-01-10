using Data.Layer.Repository;
using Logger.Layer.Log.Service;
using System.Reflection;
using System.Windows;
using Transports.Properties;

namespace Transports
{
    /// <summary>
    /// Lógica de interacción para Main.xaml
    /// </summary>
    public partial class Main : Window
    {
        public Main()
        {
            InitializeComponent();
            if (!Context.OpenConnection(Settings.Default.ConnectionString))
            {
                LoggerManager.HandleException(new System.Exception("No se pudo encontrar el archivo de datos"));
                MessageBox.Show("No se pudo encontrar el archivo de datos", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Traslates_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().ShowDialog();
        }

        private void Customers_Click(object sender, RoutedEventArgs e)
        {
            new CustomerHours().ShowDialog();
        }

        private void Drivers_Click(object sender, RoutedEventArgs e)
        {
            new DriversManager().ShowDialog();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public override void BeginInit()
        {
            Assembly.Load("System.Data.SQLite");
            base.BeginInit();
        }
    }
}
