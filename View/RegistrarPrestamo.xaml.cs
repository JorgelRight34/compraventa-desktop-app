using firstWPFApp.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
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
using System.Windows.Threading;
using MessageBox = System.Windows.Forms.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace firstWPFApp.View
{
    /// <summary>
    /// Interaction logic for RegistrarPrestamo.xaml
    /// </summary>
    public partial class RegistrarPrestamo : Window
    {
        public ObservableCollection<Cliente> clientes = new ObservableCollection<Cliente>();
        private string connectionString = "Data Source=LAPTOP-BFV33PDI\\SQLEXPRESS;" +
                                          "Initial Catalog=CompraVentaApp;Integrated Security=True";
        private string query;
        public string destinationPath;
        public string fotoSubidaPath;
        public string fotoPath;
        public Prestamo prestamo;
        bool registrar;

        public RegistrarPrestamo()
        {
            InitializeComponent();

            GetClientes();
            query = "INSERT INTO Prestamos (Id_Cliente,Precio,Tasa_Interés,Plazo,Fecha,Titulo_Articulo,Foto,Descripcion,Total_Pagado,Estado,Capital_Pagado,Interés_Pagado) " +
                    "VALUES (@Id_Cliente,@Precio,@Tasa_Interés,@Plazo,@Fecha,@Titulo_Articulo,@Foto,@Descripcion,@Total_Pagado,@Estado,@Capital_Pagado,@Interés_Pagado)";
            DateTime fechaActual = DateTime.Now;
            FechaPicker.SelectedDate = fechaActual;
            FechaPicker.DisplayDate = fechaActual;
            RegistrarPrestamoGrid.Visibility = Visibility.Visible;
            registrar = true;
        }

        public RegistrarPrestamo(Prestamo prestamo)
        {
            InitializeComponent();

            GetClientes();
            this.prestamo = prestamo;
            SubirFoto.Content = "";
            FechaPicker.SelectedDate = prestamo.Fecha;
            FechaPicker.DisplayDate = prestamo.Fecha;

            query = "UPDATE Prestamos SET Id_Cliente = @Id_Cliente, Precio = @Precio, " +
                    "Tasa_Interés = @Tasa_Interés, " +
                    "Plazo = @Plazo, " +
                    "Fecha = @Fecha, " +
                    "Titulo_Articulo = @Titulo_Articulo, " +
                    "Foto = @Foto, " +
                    "Descripcion = @Descripcion " +
                    $"WHERE Id_Prestamo = {prestamo.Id}";

            EditarPrestamoGrid.Visibility = Visibility.Visible;
            Dictionary<string, int> plazos = new Dictionary<string, int>
            {
                { "15 días", 0 }, { "1 mes", 1 }, { "2 meses", 2 }
            };
            Cliente clienteSeleccionado = clientes.FirstOrDefault(item => item.Id == prestamo.Id_Cliente);
            int indexClienteSeleccionado = clientes.IndexOf(clienteSeleccionado);
            ClienteComboBox.SelectedIndex = indexClienteSeleccionado;
            PlazoComboBox.SelectedIndex = plazos[prestamo.Plazo];
            txtTasaInteres.Text = prestamo.TasaInteres.ToString();
            txtPrecio.Text = prestamo.Precio.ToString();
            txtArticuloTitulo.Text = prestamo.Titulo;
            try
            {
                ImagenArticulo.Source = new BitmapImage(new Uri(prestamo.Foto));
            } catch (System.IO.DirectoryNotFoundException err)
            {
                Console.WriteLine(err);
            }
   
            txtDescripcion.Text = prestamo.Descripcion;
            PlazoComboBox.SelectedItem = prestamo.Plazo;
            fotoSubidaPath = prestamo.Foto;
            registrar = false;


        }

        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            if (ClienteComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Debe seleccionar un cliente.");
                ClienteComboBox.Focus();
            } 
            else if (fotoSubidaPath == null && registrar) 
            {
                MessageBox.Show("Debe subir una foto.");
            }

            else if (PlazoComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Debe seleccionar un plazo.");
                PlazoComboBox.Focus();
            }
            else if (FechaPicker.SelectedDate == null)
            {
                MessageBox.Show("Debe seleccionar una fecha.");
            }
            else if (txtDescripcion.Text == null)
            {
                MessageBox.Show("Debe ingresar una descripcion.");
            }
            else if (txtDescripcion.Text.Length > 50)
            {
                MessageBox.Show("Descripcion muy larga, no debe superar los 50 caracteres.");
            }
            else
            {
                try
                {
                    double tasaInteres = Convert.ToDouble(txtTasaInteres.Text);
                }
                catch
                {
                    MessageBox.Show("La tasa de interes debe ser un valor numerico.");
                    txtTasaInteres.Clear();
                    txtTasaInteres.Focus();
                }

                if (txtPrecio.Text == null)
                {
                    MessageBox.Show("Debe ingresar un precio.");
                    txtPrecio.Clear();
                    txtPrecio.Focus();
                }
                else
                {
                    string plazo = ((ComboBoxItem)PlazoComboBox.SelectedItem).Content.ToString();
                    Cliente cliente = (Cliente)ClienteComboBox.SelectedItem;
                    SqlConnection connection = new SqlConnection(connectionString);

                    connection.Open();
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@Id_Cliente", cliente.Id);
                    cmd.Parameters.AddWithValue("@Tasa_Interés", txtTasaInteres.Text);
                    cmd.Parameters.AddWithValue("@Precio", txtPrecio.Text);
                    cmd.Parameters.AddWithValue("@Plazo", plazo);
                    cmd.Parameters.AddWithValue("@Fecha", FechaPicker.SelectedDate.Value.ToString("MM/dd/yyyy"));
                    cmd.Parameters.AddWithValue("@Titulo_Articulo", txtArticuloTitulo.Text);
                    cmd.Parameters.AddWithValue("@Foto", fotoSubidaPath);
                    cmd.Parameters.AddWithValue("@Descripcion", txtDescripcion.Text);
                    cmd.Parameters.AddWithValue("@Total_Pagado", 0);
                    cmd.Parameters.AddWithValue("@Estado", "Activo");
                    cmd.Parameters.AddWithValue("@Capital_Pagado", 0);
                    cmd.Parameters.AddWithValue("@Interés_Pagado", 0);
                    cmd.ExecuteNonQuery();
                    connection.Close();
                    if (registrar)
                    {
                        File.Copy(fotoPath, fotoSubidaPath, true);
                    }
                    MessageBox.Show("Se ha registrado con éxito");
                    Home.GetListaPrestamos();
                    try
                    {
                        InfoCliente.GetPagos();
                    }
                    catch { };
                    Close();
                }
            }
        }

        public void GetClientes()
        {
            SqlConnection connection = new SqlConnection(connectionString);
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
            ClienteComboBox.ItemsSource = clientes; 
        }

        private void SubirFoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Image Files | *.jpg; *.jpeg; *.png; *.gif; *.bmp";
            if (fileDialog.ShowDialog() == true)
            {
                fotoPath = fileDialog.FileName;
                string directorioActual = Directory.GetCurrentDirectory();
                string fileName = System.IO.Path.GetFileName(fotoPath);
                destinationPath = System.IO.Path.Combine(directorioActual, "Imagenes");
                this.fotoSubidaPath = System.IO.Path.Combine(destinationPath, fileName);

                ImagenArticulo.Source = new BitmapImage(new Uri(fotoPath));
                SubirFoto.Content = "";
            }
        }

        private void Borrar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult decision = (MessageBoxResult)MessageBox.Show("¿Seguro que desea borrar?, todos los datos relacionados serán eliminados", "Confirmación de Borrado", (MessageBoxButtons)MessageBoxButton.YesNo, (MessageBoxIcon)MessageBoxImage.Warning);
            if (decision == MessageBoxResult.Yes)
            {
                query = $"DELETE FROM Prestamos WHERE Id_Prestamo = {prestamo.Id}";
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand(query, connection);
                connection.Open();
                cmd.ExecuteNonQuery();
                Home.GetListaPrestamos();
                try
                {
                    InfoCliente.GetPrestamos();
                }
                catch { };

                query = $"DELETE FROM Pagos WHERE Id_Prestamo = {prestamo.Id}";
                cmd = new SqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                Home.GetListaPrestamos();
                try
                {
                    InfoCliente.GetPrestamos();
                }
                catch { };
                connection.Close();
                Close();
            }
            else
            {

            }          
        }
    }
}
