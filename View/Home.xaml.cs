using firstWPFApp.Models;
using firstWPFApp.View.UserControls;
using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.Win32;
using OfficeOpenXml.Style;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using Button = System.Windows.Controls.Button;
using MessageBox = System.Windows.Forms.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace firstWPFApp.View
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Window
    {
        public static ObservableCollection<Prestamo> prestamos = new ObservableCollection<Prestamo>();
        public static ObservableCollection<Prestamo> prestamosPasados = new ObservableCollection<Prestamo>();
        public static ObservableCollection<Cliente> clientes = new ObservableCollection<Cliente>();
        public static ObservableCollection<Producto> productos = new ObservableCollection<Producto>();
        public static ObservableCollection<Pago> pagos = new ObservableCollection<Pago>();
        public static ObservableCollection<Venta> ventas = new ObservableCollection<Venta>();
        private Usuario usuario;
        public static SqlConnection connection;
        public string connectionString = "Data Source=LAPTOP-BFV33PDI\\" +
                                          "SQLEXPRESS;Initial Catalog=CompraVentaApp;" +
                                          "Integrated Security=True";
        private string botonActivo;
        private string registrosActuales;

        public Home(Usuario usuario)
        {
            InitializeComponent();

            WindowState = WindowState.Maximized;
            this.usuario = usuario;
            nombreUsuario.Text = usuario.Nombre;
            connection = new SqlConnection(connectionString);
        }

        public static void GetListaPrestamos()
        {
            GetListaPagos();
            connection.Open();
            string query = "SELECT * FROM Prestamos";
            SqlCommand cmd = new SqlCommand(query, connection);
            SqlDataReader reader = cmd.ExecuteReader();

            prestamos.Clear();
            prestamosPasados.Clear();
            while (reader.Read())
            {
                Prestamo prestamo = new Prestamo
                {
                    Id = (int)reader["Id_Prestamo"],
                    Id_Cliente = (int)reader["Id_Cliente"],
                    Titulo = (string)reader["Titulo_Articulo"],
                    Descripcion = (string)reader["Descripcion"],
                    Precio = (double)reader["Precio"],
                    Foto = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, (string)reader["Foto"]),
                    TasaInteres = (double)reader["Tasa_Interés"],
                    Fecha = DateTime.Parse((string)reader["Fecha"]),
                    Plazo = (string)reader["Plazo"],
                    Estado = (string)reader["Estado"],
                    CapitalPagado = (double)reader["Capital_Pagado"],
                    InteresPagado = (double)reader["Interés_Pagado"],
                    NombreCliente = Cliente.GetCliente((int)reader["Id_Cliente"]).Nombre,
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
            }
            connection.Close();   
        }

        public static void GetListaProductos()
        {
            connection.Open();
            string query = "SELECT * FROM Productos";
            SqlCommand cmd = new SqlCommand(query, connection);
            SqlDataReader reader = cmd.ExecuteReader();

            productos.Clear();
            while (reader.Read())
            {
                Producto producto = new Producto
                {
                    Id = (int)reader["Id_Producto"],
                    Titulo = (string)reader["Titulo"],
                    Descripcion = (string)reader["Descripcion"],
                    Precio = reader["Precio"].ToString(),
                    Foto = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, (string)reader["Foto"]),
                };
                productos.Add(producto);
            }
            connection.Close();
        }

        public static void GetListaClientes()
        {
            connection.Open();
            string query = "SELECT * FROM Clientes";
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
                    Genero = (string)reader["Género"],
                    FechaNacimiento = (string)reader["Fecha_Nacimiento"],
                    Telefono = (string)reader["Teléfono"],

                };
                clientes.Add(cliente);
            }
            connection.Close();
        }
        public static void GetListaPagos()
        {
            connection.Open();
            string query = "SELECT * FROM Pagos";
            SqlCommand cmd = new SqlCommand(query, connection);
            SqlDataReader reader = cmd.ExecuteReader();

            pagos.Clear();
            while (reader.Read())
            {
                Pago pago = new Pago
                {
                    Id = (int)reader["Id_Pago"],
                    Id_Prestamo = (int)reader["Id_Prestamo"],
                    Monto = (double)reader["Monto"],
                    Fecha = DateTime.Parse((string)reader["Fecha_Pago"]),
                    Capital_Pagado = (double)reader["Capital_Pagado"],
                    Interes_Pagado = (double)reader["Interés_Pagado"],
                };
                pagos.Add(pago);
            }
            connection.Close();
        }
        public static void GetListaVentas()
        {
            connection.Open();
            string query = "SELECT * FROM Ventas";
            SqlCommand cmd = new SqlCommand(query, connection);
            SqlDataReader reader = cmd.ExecuteReader();

            ventas.Clear();
            while (reader.Read())
            {
                Venta venta = new Venta
                {
                    Id = (int)reader["Id_Venta"],
                    Id_Producto = (int)reader["Id_Producto"],
                    Id_Cliente = (int)reader["Id_Cliente"],
                    Monto = (double)reader["Monto"],
                    Fecha = DateTime.Parse((string)reader["Fecha"]),
                    Foto = (string)reader["Foto"],
                    Descripcion = (string)reader["Descripción"],
                };
                ventas.Add(venta);
            }
            connection.Close();
        }
        private void ApagarVisibilidades()
        {
            lstInventario_ScrollViewer.Visibility = Visibility.Hidden;
            lstInventario.Visibility = Visibility.Hidden;
            //InventarioDataGrid.Visibility = Visibility.Hidden;
            InventarioGrid.Visibility = Visibility.Hidden;
            lstClientes_ScrollViewer.Visibility = Visibility.Hidden;
            lstClientes.Visibility = Visibility.Hidden;
            //ClientesDataGrid.Visibility = Visibility.Hidden;
            ClientesGrid.Visibility = Visibility.Hidden;
            lstPrestamos_ScrollViewer.Visibility = Visibility.Hidden;
            lstPrestamos.Visibility = Visibility.Hidden;
            //PrestamosDataGrid.Visibility = Visibility.Hidden;
            PrestamosGrid.Visibility = Visibility.Hidden;
            lstPrestamosPago_ScrollViewer.Visibility = Visibility.Hidden;
            lstPrestamosPago.Visibility = Visibility.Hidden;
            //PagosDataGrid.Visibility = Visibility.Hidden;
            PagosGrid.Visibility = Visibility.Hidden;
            //lstPagos_ScrollViewer.Visibility = Visibility.Hidden;
            //lstPagos.Visibility = Visibility.Hidden;
            lstPrestamosPasados_ScrollViewer.Visibility = Visibility.Hidden;
            lstPrestamosPasados.Visibility = Visibility.Hidden;
            //PrestamosPasadosDataGrid.Visibility = Visibility.Hidden;
            PrestamosPasadosGrid.Visibility = Visibility.Hidden;
            //VentasDataGrid.Visibility = Visibility.Hidden;
            lstVentas_ScrollViewer.Visibility = Visibility.Hidden;
            lstVentas.Visibility = Visibility.Hidden;
            VentasGrid.Visibility = Visibility.Hidden;
            //lstVentas_ScrollViewer.Visibility = Visibility.Hidden;
            //lstVentas.Visibility = Visibility.Hidden;
            VistaNormal.Visibility = Visibility.Hidden;
        }
        private void RegistrarPago_Click(object sender, RoutedEventArgs e)
        {
            GetListaPrestamos();
            lstPrestamosPago.ItemsSource = prestamos;
            ApagarVisibilidades();
            lstPrestamosPago_ScrollViewer.Visibility = Visibility.Visible;
            lstPrestamosPago.Visibility = Visibility.Visible;
        }

        private void Registrar_Prestamo_Click(object sender, RoutedEventArgs e)
        {
            RegistrarPrestamo registrarPrestamoWindow = new RegistrarPrestamo();
            registrarPrestamoWindow.Show();

        }

        private void VerPrestamos_Click(object sender, RoutedEventArgs e)
        {
            GetListaPrestamos();
            ApagarVisibilidades();
            lstPrestamos.ItemsSource = prestamos;
            lstPrestamos_ScrollViewer.Visibility = Visibility.Visible;  
            lstPrestamos.Visibility = Visibility.Visible;


            VistaTabla.Visibility = Visibility.Visible;
            botonActivo = "Ver Empeños";
        }

        private void Inventario_Click(object sender, RoutedEventArgs e)
        {
            GetListaProductos();
            lstInventario.ItemsSource = productos;
            ApagarVisibilidades();
            lstInventario_ScrollViewer.Visibility = Visibility.Visible;
            lstInventario.Visibility = Visibility.Visible;


            VistaTabla.Visibility = Visibility.Visible;
            botonActivo = "Inventario";
        }


        private void VerClientes_Click(object sender, RoutedEventArgs e)
        {
            GetListaClientes();
            ApagarVisibilidades();
            ClientesGrid.Visibility = Visibility.Visible;
            ClientesCount.Text = clientes.Count().ToString();
            ClientesDataGrid.ItemsSource = clientes;
            ClientesDataGrid.Visibility = Visibility.Visible;

            /*ApagarVisibilidades();
            lstClientes.ItemsSource = clientes;
            lstClientes_ScrollViewer.Visibility = Visibility.Visible;
            lstClientes.Visibility = Visibility.Visible;


            VistaTabla.Visibility = Visibility.Visible;*/
            VistaTabla.Visibility = Visibility.Hidden;
            botonActivo = "Ver Clientes";
        }

        private void RegistrarCliente_Click(object sender, RoutedEventArgs e)
        {
            RegistrarCliente registrarClienteWindow = new RegistrarCliente();
            registrarClienteWindow.Show();
        }

        private void AgregarInventario_Click(object sender, RoutedEventArgs e)
        {
            AgregarArticulo agregarArticuloWindow = new AgregarArticulo();
            agregarArticuloWindow.Show();
        }

        private void CerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            IniciarSesion iniciarSesionWindow = new IniciarSesion();
            iniciarSesionWindow.Show();
            Close();
        }

        private void Usuario_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Pagos_Click(object sender, RoutedEventArgs e)
        {
            GetListaPagos();
            ApagarVisibilidades();
            PagosDataGrid.ItemsSource = pagos;
            PagosGrid.Visibility = Visibility.Visible;

            VistaTabla.Visibility = Visibility.Hidden;
            botonActivo = "Pagos";
            PagosCount.Text = pagos.Count().ToString();
        }

        private void EditarPago_Click(object sender, RoutedEventArgs e)
        {
            
            Pago pago = (Pago)PagosDataGrid.SelectedItem;
            RegistrarPago registrarPagoWindow = new RegistrarPago(pago);
            registrarPagoWindow.Show();
            GetListaPagos();
      

     
        }

        private void VerPrestamosPasados_Click(object sender, RoutedEventArgs e)
        {
            ApagarVisibilidades();
            lstPrestamosPasados_ScrollViewer.Visibility = Visibility.Visible;
            lstPrestamosPasados.Visibility = Visibility.Visible;
            GetListaPrestamos();
            lstPrestamosPasados.ItemsSource = prestamosPasados;

            VistaTabla.Visibility = Visibility.Visible;
            botonActivo = "Empeños Pasados";
        }

        private void Ventas_Click(object sender, RoutedEventArgs e)
        {
            ApagarVisibilidades();
            GetListaVentas();
            lstVentas.ItemsSource = ventas;
            lstVentas_ScrollViewer.Visibility = Visibility.Visible;
            lstVentas.Visibility = Visibility.Visible;
            

            botonActivo = "Ventas";
        }

        private void EditarVenta_Click(object sender, RoutedEventArgs e)
        {
            Venta venta = (Venta)VentasDataGrid.SelectedItem;
            VenderArticulo venderArticuloWindow = new VenderArticulo(venta);
            venderArticuloWindow.Show();
        }
        
        private void Buscar_Click(object sender, RoutedEventArgs e)
        {
            string query;
            switch (botonActivo)
            {
                case ("Ver Empeños"):
                    query = "SELECT * FROM Prestamos WHERE CONCAT([Id_Prestamo], [Id_Cliente], [Precio], [Tasa_Interés], " +
                            "[Plazo], [Fecha], [Titulo_Articulo], [Foto], [Descripcion], [Total_Pagado], [Estado]) " +
                            "LIKE '%" + txtBuscar.Text + "%'";
                    Prestamo.Buscar(query);
                    return;

                case ("Empeños Pasados"):
                    query = "SELECT * FROM Prestamos WHERE CONCAT([Id_Prestamo], [Id_Cliente], [Precio], [Tasa_Interés], " +
                            "[Plazo], [Fecha], [Titulo_Articulo], [Foto], [Descripcion], [Total_Pagado], [Estado]) " +
                            "LIKE '%" + txtBuscar.Text + "%'" + " AND Estado = 'Saldado'";
                    Prestamo.Buscar(query);
                    return;

                case ("Pagos"):
                   PagosDataGrid.DataContext = Pago.Buscar(txtBuscar.Text);
                    return;

                case ("Ver Clientes"):
                    Cliente.Buscar(txtBuscar.Text);
                    return;

                case ("Inventario"):
                    Producto.Buscar(txtBuscar.Text);
                    return;

                case ("Ventas"):
                    Venta.Buscar(txtBuscar.Text);
                    return;
            }
        }

        private void VistaTabla_Click(object sender, RoutedEventArgs e)
        {
            ApagarVisibilidades();
            VistaTabla.Visibility = Visibility.Hidden;
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

                case ("Inventario"):
                    InventarioDataGrid.ItemsSource = productos;
                    InventarioGrid.Visibility = Visibility.Visible;
                    InventarioDataGrid.Visibility = Visibility.Visible;
                    InventarioCount.Text = productos.Count().ToString();
                    return;

                case ("Ver Clientes"):
                    ClientesDataGrid.ItemsSource = clientes;
                    ClientesGrid.Visibility = Visibility.Visible;
                    ClientesDataGrid.Visibility = Visibility.Visible;
                    ClientesCount.Text = clientes.Count().ToString();
                    return;

                case ("Ventas"):
                    VentasDataGrid.ItemsSource = ventas;
                    VentasGrid.Visibility = Visibility.Visible;
                    VentasDataGrid.Visibility = Visibility.Visible; 
                    VistaTabla.Visibility = Visibility.Hidden;
                    VentasCount.Text = ventas.Count.ToString();
                    return;
            }
        }

        private void GenerarRegistros_Click(object sender, RoutedEventArgs e)
        {

            using (var dialog = new FolderBrowserDialog())
            {
                DialogResult result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    string selectedFolderPath = dialog.SelectedPath;
                    GenerarRegistros.GenerarRegistro(selectedFolderPath);
                    MessageBox.Show("Los registros han sido guardados exitosaménte");
                }

            }
        }
        private void EditarPrestamo_Click(object sender, RoutedEventArgs e)
        {
            Prestamo prestamo = (Prestamo)PrestamosDataGrid.SelectedItem;
            RegistrarPrestamo editarPrestamoWindow = new RegistrarPrestamo(prestamo);
            editarPrestamoWindow.Show();
        }

        private void EditarProducto_Click(object sender, RoutedEventArgs e)
        {
            Producto producto = (Producto)InventarioDataGrid.SelectedItem;  
            AgregarArticulo agregarArticuloWindow = new AgregarArticulo(producto);
            agregarArticuloWindow.Show();
            GetListaProductos();
        }

        private void EditarCliente_Click(object sender, RoutedEventArgs e)
        {
            Cliente cliente = (Cliente)ClientesDataGrid.SelectedItem;
            InfoCliente infoClienteWindow = new InfoCliente(cliente);
            infoClienteWindow.Show();
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

                case ("Inventario"):
                    lstInventario_ScrollViewer.Visibility = Visibility.Visible;
                    lstInventario.Visibility = Visibility.Visible;
                    VistaTabla.Visibility = Visibility.Visible;
                    return;

                case ("Ver Clientes"):
                    lstClientes_ScrollViewer.Visibility = Visibility.Visible;
                    lstClientes.Visibility = Visibility.Visible;
                    VistaTabla.Visibility = Visibility.Visible;
                    return;
            }
        }

        private void txtBuscar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtBuscar.Text = "";
        }
    }
}
