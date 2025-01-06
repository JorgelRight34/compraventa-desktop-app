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
    /// Interaction logic for PrestamoItem.xaml
    /// </summary>
    public partial class PrestamoItem : UserControl
    {
        private Prestamo prestamo { get { return DataContext as Prestamo; } }
        public PrestamoItem()
        {
            InitializeComponent();
           // labelDiasRestantes.Content = CalcularFechaPago().ToString();
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

        private TimeSpan CalcularFechaPago()
        {
            Dictionary<string, int> plazos = new Dictionary<string, int>
            {
                {"15 dias", 15 }, {"1 mes", 31}, {"2 meses", 62}, {"3 meses", 93}
            };
            List<DateTime> fechaPagos = new List<DateTime>();

            foreach (Prestamo prestamo in Home.prestamos)
            {
                fechaPagos.Add(prestamo.Fecha);
            }
            DateTime ultimaFecha = fechaPagos.Max();
            int plazo = plazos[prestamo.Plazo];
            DateTime fechaPago = ultimaFecha.AddDays(plazo);
            TimeSpan diasRestantes = DateTime.Now - fechaPago;
            return diasRestantes;
        }
    }
}
