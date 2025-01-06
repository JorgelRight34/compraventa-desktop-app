using firstWPFApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace firstWPFApp.View.UserControls
{
    /// <summary>
    /// Interaction logic for Prestamo.xaml
    /// </summary>
    public partial class PrestamoView : UserControl
    {
        private Prestamo prestamo { get { return DataContext as Prestamo; } }
        public PrestamoView()
        {
            InitializeComponent();
            
        }

        private void EditarPrestamo_Click(object sender, RoutedEventArgs e)
        {
            RegistrarPrestamo editarPrestamoWindow = new RegistrarPrestamo(prestamo);
            editarPrestamoWindow.Show();
        }

        private void Pagar_Click(object sender, RoutedEventArgs e)
        {
            RegistrarPago registrarPagoWindow = new RegistrarPago(prestamo);
            registrarPagoWindow.Show();
        }
    }
}
