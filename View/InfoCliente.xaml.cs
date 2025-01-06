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
using static Microsoft.IO.RecyclableMemoryStreamManager;

namespace firstWPFApp.View
{
    /// <summary>
    /// Interaction logic for InfoCliente.xaml
    /// </summary>
    public partial class InfoCliente : Window
    {
        private Cliente cliente;
        private static int Id_Cliente;
        private static string connectionString = "Data Source=LAPTOP-BFV33PDI\\" +
                                          "SQLEXPRESS;Initial Catalog=CompraVentaApp;" +
                                          "Integrated Security=True";
        public static ObservableCollection<Prestamo> prestamos = new ObservableCollection<Prestamo>();
        public static ObservableCollection<Prestamo> prestamosPasados = new ObservableCollection<Prestamo>();
        public static ObservableCollection<Pago> pagos = new ObservableCollection<Pago>();
        public static ObservableCollection<Venta> compras = new ObservableCollection<Venta>();
        public static SqlConnection connection = new SqlConnection(connectionString);
        private static string botonActivo;
        public InfoCliente(Cliente cliente)
        {
            InitializeComponent();
            this.cliente = cliente;
            Id_Cliente = cliente.Id;
            nombreCliente.Text = cliente.Nombre;

        }

        private void ApagarVisibilidades()
        {
            //lstCompras_ScrollViewer.Visibility = Visibility.Hidden;
            //lstCompras.Visibility = Visibility.Hidden;
            lstPrestamos_ScrollViewer.Visibility = Visibility.Hidden;
            lstPrestamos.Visibility = Visibility.Hidden;
            ComprasDataGrid.Visibility = Visibility.Hidden;
            PagosDataGrid.Visibility = Visibility.Hidden;
            lstPrestamosPasados_ScrollViewer.Visibility = Visibility.Hidden;
            lstPrestamosPasados.Visibility = Visibility.Hidden;
            PrestamosPasadosDataGrid.Visibility = Visibility.Hidden;

            PrestamosGrid.Visibility = Visibility.Hidden;
            ComprasGrid.Visibility = Visibility.Hidden;
            PagosGrid.Visibility = Visibility.Hidden;
            PrestamosPasadosGrid.Visibility = Visibility.Hidden;
            VistaTabla.Visibility = Visibility.Hidden;
            VistaNormal.Visibility = Visibility.Hidden;

            lstCompras_ScrollViewer.Visibility = Visibility.Hidden;
            lstCompras.Visibility = Visibility.Hidden;
            //lstPagos_ScrollViewer.Visibility = Visibility.Hidden;
            //lstPagos.Visibility = Visibility.Visible;
        }

        public static void GetCompras()
        {
            string query = $"SELECT * FROM Ventas WHERE Id_Cliente = {Id_Cliente}";
            compras.Clear();
            connection.Open();
            SqlCommand cmd = new SqlCommand(query, connection);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Venta compra = new Venta
                {
                    Id = (int)reader["Id_Venta"],
                    Id_Producto = (int)reader["Id_Producto"],
                    Id_Cliente = (int)reader["Id_Cliente"],
                    Monto = (double)reader["Monto"],
                    Fecha = DateTime.Parse((string)reader["Fecha"]),
                    Foto = (string)reader["Foto"],
                    Descripcion = (string)reader["Descripcion"],
                };
                compras.Add(compra);
            }
            connection.Close();
        }
        
        public static void GetPagos()
        {
            pagos.Clear();
            connection.Open();
            foreach (Prestamo prestamo in prestamos)
            {
                string query = $"SELECT * FROM Pagos WHERE Id_Prestamo = {prestamo.Id}";
                SqlCommand cmd = new SqlCommand(query, connection);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Pago pago = new Pago
                    {
                        Id = (int)reader["Id_Pago"],
                        Id_Prestamo = (int)reader["Id_Pago"],
                        Monto = (double)reader["Monto"],
                        Fecha = DateTime.Parse((string)reader["Fecha_Pago"]),
                        Capital_Pagado = (double)reader["Capital_Pagado"],
                        Interes_Pagado = (double)reader["Interés_Pagado"],
                    };
                    pagos.Add(pago);
                }
            }
            connection.Close();
        }

        public static void GetPrestamos()
        {
            string query = $"SELECT * FROM Prestamos WHERE Id_Cliente = {Id_Cliente}";
            prestamos.Clear();
            connection.Open();
            SqlCommand cmd = new SqlCommand(query, connection);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Prestamo prestamo = new Prestamo
                {
                    Id = (int)reader["Id_Prestamo"],
                    Titulo = (string)reader["Titulo_Articulo"],
                    Descripcion = (string)reader["Descripcion"],
                    Precio = (double)reader["Precio"],
                    Foto = (string)reader["Foto"],
                    TasaInteres = (double)reader["Tasa_Interés"],
                    Fecha = DateTime.Parse((string)reader["Fecha"]),
                    Plazo = (string)reader["Plazo"],
                    Estado = (string)reader["Estado"],
                    CapitalPagado = (double)reader["Capital_Pagado"],
                    InteresPagado = (double)reader["Interés_Pagado"],
                };
                prestamo.CalcularFechaPago();
                prestamo.CalcularUltimoPago();
                if (prestamo.Estado.Trim().Equals("Saldado"))
                {
                    prestamosPasados.Add(prestamo);
                }
                else
                {
                    prestamos.Add(prestamo);
                }
                prestamos.Add(prestamo);
            }
            connection.Close();
        }
        private void VerCompras_Click(object sender, RoutedEventArgs e)
        {
            ApagarVisibilidades();
            GetCompras();
            botonActivo = "Compras";
            lstCompras_ScrollViewer.Visibility = Visibility.Visible;
            lstCompras.Visibility = Visibility.Visible;
            ComprasCount.Text = compras.Count().ToString();
            //ComprasDataGrid.Visibility = Visibility.Visible;
            //lstCompras_ScrollViewer.Visibility = Visibility.Visible;
            //lstCompras.Visibility = Visibility.Visible;
        }

        private void Pagos_Click(object sender, RoutedEventArgs e)
        {
            ApagarVisibilidades();
            GetPagos();
            PagosDataGrid.Visibility = Visibility.Visible;
            PagosCount.Text = pagos.Count().ToString();
            botonActivo = "Pagos";

            //lstPagos.ItemsSource = pagos;
            //lstPagos_ScrollViewer.Visibility = Visibility.Visible;
            //lstPagos.Visibility = Visibility.Visible;
        }

        private void VerPrestamos_Click(object sender, RoutedEventArgs e)
        {
            ApagarVisibilidades();
            GetPrestamos();
            lstPrestamos.ItemsSource = prestamos;
            lstPrestamos_ScrollViewer.Visibility = Visibility.Visible;
            lstPrestamos.Visibility = Visibility.Visible;
            botonActivo = "Ver Empeños";
        }

        private void Cliente_Click(object sender, RoutedEventArgs e)
        {
            RegistrarCliente registrarClienteWindow = new RegistrarCliente(cliente);
            registrarClienteWindow.Show();
        }
        private void EditarCompra_Click(object sender, RoutedEventArgs e)
        {
            var venta = (sender as FrameworkElement).DataContext as Venta;
            VenderArticulo venderArticuloWindow = new VenderArticulo(venta);
            venderArticuloWindow.Show();
        }

        private void Borrar_Click(object sender, RoutedEventArgs e)
        {
            var venta = (sender as FrameworkElement).DataContext as Venta;
            string query = $"DELETE FROM Ventas WHERE Id_Venta = {venta.Id}";
            connection.Open();
            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.ExecuteNonQuery();
            GetCompras();
        }

        private void EditarPago_Click(object sender, RoutedEventArgs e)
        {
            //Button botonClickeado = (Button)sender;
            //var dataGrid = (DataGrid)botonClickeado.Content;
            //Pago pago = (Pago)dataGrid.DataContext;
            var pago = (sender as FrameworkElement).DataContext as Pago;
            RegistrarPago registrarPagoWindow = new RegistrarPago(pago);
            registrarPagoWindow.Show();
        }
        private void Buscar_Click(object sender, RoutedEventArgs e)
        {
            string query;
            switch (botonActivo)
            {
                case ("Ver Empeños"):
                    query = "SELECT * FROM Prestamos WHERE CONCAT([Id_Prestamo], [Id_Cliente], [Precio], [Tasa_Interes], " +
                            "[Plazo], [Fecha], [Titulo_Articulo], [Foto], [Descripcion], [Total_Pagado], [Estado]) " +
                            "LIKE '%" + txtBuscar.Text + "%'" + $" WHERE Id_Cliente = {cliente.Id}";
                    Prestamo.Buscar(query);
                    return;

                case ("Empeños Pasados"):
                    query = "SELECT * FROM Prestamos WHERE CONCAT([Id_Prestamo], [Id_Cliente], [Precio], [Tasa_Interes], " +
                           "[Plazo], [Fecha], [Titulo_Articulo], [Foto], [Descripcion], [Total_Pagado], [Estado]) " +
                           "LIKE '%" + txtBuscar.Text + "%'" + " WHERE Estado = 'Saldado'" +
                           $" AND WHERE Id_Cliente = {cliente.Id}";
                    Prestamo.Buscar(query);
                    return;

                case ("Pagos"):
                    PagosDataGrid.DataContext = Pago.Buscar(txtBuscar.Text, cliente.Id);
                    return;
                    
                case ("Inventario"):
                    Producto.Buscar(txtBuscar.Text, cliente.Id);
                    return;

                case ("Compras"):
                    ComprasDataGrid.DataContext = Venta.Buscar(txtBuscar.Text, cliente.Id);
                    return;
            }
        }

        private void VistaTabla_Click(object sender, RoutedEventArgs e)
        {
            ApagarVisibilidades();
            VistaNormal.Visibility = Visibility.Visible;
            switch (botonActivo)
            {
                case ("Ver Empeños"):
                    PrestamosDataGrid.ItemsSource = prestamos;
                    PrestamosGrid.Visibility = Visibility.Visible;
                    PrestamosDataGrid.Visibility = Visibility.Visible;
                    PrestamosCount.Text = prestamos.Count().ToString();
                    return;

                case ("Empeños Pasados"):
                    PrestamosPasadosDataGrid.ItemsSource = prestamosPasados;
                    PrestamosPasadosGrid.Visibility = Visibility.Visible;
                    PrestamosDataGrid.Visibility = Visibility.Visible;
                    PrestamosPasadosCount.Text = prestamosPasados.Count().ToString();
                    return;

                case ("Compras"):
                    ComprasDataGrid.ItemsSource = compras;
                    ComprasGrid.Visibility = Visibility.Visible;
                    ComprasDataGrid.Visibility = Visibility.Visible;
                    VistaTabla.Visibility = Visibility.Hidden;
                    ComprasCount.Text = compras.Count.ToString();
                    return;
            }
        }

        private void VistaNormal_Click(object sender, RoutedEventArgs e)
        {
            ApagarVisibilidades();
            switch (botonActivo)
            {
                case ("Ver Empeños"):
                    lstPrestamos_ScrollViewer.Visibility = Visibility.Visible;
                    lstPrestamos.Visibility = Visibility.Visible;
                    VistaTabla.Visibility = Visibility.Visible;
                    return;

                case ("Empeños Pasados"):
                    lstPrestamosPasados_ScrollViewer.Visibility = Visibility.Visible;
                    lstPrestamosPasados.Visibility = Visibility.Visible;
                    VistaTabla.Visibility = Visibility.Visible;
                    return;
            }
        }

        private void PrestamosPasados_Click(object sender, RoutedEventArgs e)
        {
            ApagarVisibilidades();
            lstPrestamosPasados_ScrollViewer.Visibility = Visibility.Visible;
            lstPrestamosPasados.Visibility = Visibility.Visible;
            GetPrestamos();
            lstPrestamosPasados.ItemsSource = prestamosPasados;
            botonActivo = "Empeños Pasados";
        }

        private void EditarPrestamo_Click(object sender, RoutedEventArgs e)
        {
            Prestamo prestamo = (Prestamo)PrestamosDataGrid.SelectedItem;
            RegistrarPrestamo editarPrestamoWindow = new RegistrarPrestamo(prestamo);
            editarPrestamoWindow.Show();
        }

        private void EditarVenta_Click(object sender, RoutedEventArgs e)
        {
            Venta compra = (Venta)ComprasDataGrid.SelectedItem;
            VenderArticulo venderArticuloWindow = new VenderArticulo(compra);
            venderArticuloWindow.Show();
        }

    }
}
