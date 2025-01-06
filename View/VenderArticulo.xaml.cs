using firstWPFApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace firstWPFApp.View
{
    /// <summary>
    /// Interaction logic for VenderArticulo.xaml
    /// </summary>
    public partial class VenderArticulo : Window
    {
        public ObservableCollection<Cliente> clientes = new ObservableCollection<Cliente>();
        private Producto producto;
        private static string connectionString = "Data Source=LAPTOP-BFV33PDI\\SQLEXPRESS;" +
                                          "Initial Catalog=CompraVentaApp;Integrated Security=True";
        private SqlConnection connection = new SqlConnection(connectionString);
        private string query;
        private Venta venta;
        private int Id_Producto;
        
        public VenderArticulo(Producto producto)
        {
            InitializeComponent();
            GetClientes();
            this.producto = producto;
            query = "INSERT INTO Ventas (Id_Producto,Id_Cliente,Monto,Foto,Descripción,Fecha) " +
                    "VALUES (@Id_Producto,@Id_Cliente,@Monto,@Foto,@Descripción,@Fecha)";
            Id_Producto = producto.Id;
            Guardar.Visibility = Visibility.Visible;     
        }
        public VenderArticulo(Venta venta)
        {
            InitializeComponent();
            GetClientes();
            this.venta = venta;
            query = "UPDATE Ventas SET Id_Cliente = @Id_Cliente, Fecha = @Fecha, Monto = @Monto " +
                    $"WHERE Id_Venta = {venta.Id} ";
            Cliente clienteSeleccionado = clientes.FirstOrDefault(item => item.Id == venta.Id_Cliente);
            int indexClienteSeleccionado = clientes.IndexOf(clienteSeleccionado);
            cbClientes.SelectedIndex = indexClienteSeleccionado;
            txtMonto.Text = venta.Monto.ToString();
            fechaVenta.SelectedDate = venta.Fecha;
            fechaVenta.DisplayDate = venta.Fecha;
            Id_Producto = venta.Id_Producto;
            editarVentaGrid.Visibility = Visibility.Visible;
        }
        public void GetClientes()
        {
            string query = "SELECT * FROM Clientes";
            connection.Open();
            SqlCommand cmd = new SqlCommand(query, connection);
            SqlDataReader reader = cmd.ExecuteReader();

            clientes.Clear();
            while (reader.Read())
            {
                Cliente cliente = new Cliente
                {
                    Id = (int)reader["Id_Cliente"],
                    Nombre = (string)reader["Nombre_Completo"],
                    Email = (string)reader["Email"],
                    Cedula = (string)reader["Cédula"],
                };
                clientes.Add(cliente);
            }
            cbClientes.ItemsSource = clientes;
            connection.Close();
        }

        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            if (cbClientes.SelectedIndex == -1)
            {
                MessageBox.Show("Debe elegir un cliente");
            }
            else if (fechaVenta.SelectedDate == null)
            {
                MessageBox.Show("Debe seleccionar una fecha");
            }
            else
            {
                Cliente cliente = (Cliente)cbClientes.SelectedItem;
                DateTime fecha = fechaVenta.SelectedDate.Value;

                connection.Open();
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Id_Producto", Id_Producto);
                cmd.Parameters.AddWithValue("@Id_Cliente", cliente.Id);
                cmd.Parameters.AddWithValue("@Monto", txtMonto.Text);
                cmd.Parameters.AddWithValue("@Foto", producto.Foto);
                cmd.Parameters.AddWithValue("@Descripción", producto.Descripcion);
                cmd.Parameters.AddWithValue("@Fecha", fecha.ToString("MM/dd/yyyy"));
                cmd.ExecuteNonQuery();
                connection.Close();
                AgregarArticulo.Borrar(Id_Producto);
                Home.GetListaVentas();
                Home.GetListaProductos();
                try
                {
                    InfoCliente.GetPagos();
                }
                catch { };
                Close();
            }
        }

        private void Borrar_Click(object sender, RoutedEventArgs e)
        {
            query = $"DELETE FROM Ventas WHERE Id_Ventas = {venta.Id}";
            connection.Open();
            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.ExecuteNonQuery();
            Home.GetListaProductos();
            try
            {
                InfoCliente.GetPagos();
            }
            catch { };
            Close();
        }
    }
}
