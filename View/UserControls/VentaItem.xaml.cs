using firstWPFApp.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
    /// Interaction logic for VentaItem.xaml
    /// </summary>
    public partial class VentaItem : UserControl
    {
        private Venta venta { get { return DataContext as Venta; } }
        public VentaItem()
        {
            InitializeComponent();
        }

        private void EditarVenta_Click(object sender, RoutedEventArgs e)
        {
            VenderArticulo venderArticuloWindow = new VenderArticulo(venta);
            venderArticuloWindow.Show();
        }

        /*private void Borrar_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = "Data Source=LAPTOP-BFV33PDI\\SQLEXPRESS;" +
                                      "Initial Catalog=CompraVentaApp;Integrated Security=True";
            SqlConnection connection = new SqlConnection(connectionString);
            string query = $"DELETE FROM Ventas WHERE Id_Venta = {venta.Id}";
            connection.Open();
            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.ExecuteNonQuery();
            Home.GetListaVentas();
        }*/
    }
}
