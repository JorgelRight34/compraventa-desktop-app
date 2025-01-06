using firstWPFApp.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using MessageBox = System.Windows.Forms.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace firstWPFApp.View
{
    /// <summary>
    /// Interaction logic for AgregarArticulo.xaml
    /// </summary>
    public partial class AgregarArticulo : Window
    {
        public string? fotoSubidaPath;
        private string destinationPath;
        private string fotoPath;
        public static string connectionString = "Data Source=LAPTOP-BFV33PDI\\" +
                                         "SQLEXPRESS;Initial Catalog=CompraVentaApp;Integrated Security=True";
        public Producto articulo;
        public string query;
        public bool seleccionarFoto = false;

        public AgregarArticulo()
        {
            InitializeComponent();
            RegistrarArticuloGrid.Visibility = Visibility.Visible;
            query = "INSERT INTO Productos (Titulo,Descripcion,Precio,Foto) VALUES (@Titulo,@Descripcion,@Precio,@Foto)";
        }

        public AgregarArticulo(Producto articulo)
        {
            InitializeComponent();

            this.articulo = articulo;
            EditarArticuloGrid.Visibility = Visibility.Visible;
            query = "UPDATE Productos SET " +
                "Titulo = @Titulo, " +
                "Descripcion = @Descripcion," +
                "Precio = @Precio," +
                $"Foto = @Foto WHERE Id_Producto = {articulo.Id}";

            txtTitulo.Text = articulo.Titulo;
            txtDescripcion.Text = articulo.Descripcion;
            txtPrecio.Text = articulo.Precio.ToString();
            fotoSubidaPath = articulo.Foto;
            ImagenArticulo.Source = new BitmapImage(new Uri(articulo.Foto));
        }

        private void SubirFotoBoton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";

            if (fileDialog.ShowDialog() == true)
            {
                fotoPath = fileDialog.FileName;

                string directorioActual = Directory.GetCurrentDirectory();
                string fileName = System.IO.Path.GetFileName(fotoPath);
                destinationPath = System.IO.Path.Combine("/", "Imagenes");
                this.fotoSubidaPath = System.IO.Path.Combine(destinationPath, fileName);

                ImagenArticulo.Source = new BitmapImage(new Uri(fotoPath));
                seleccionarFoto = true;
                SubirFoto.Content = "";
            }
        }

        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            try
            {
                if (txtTitulo.Text.Equals(""))
                {
                    MessageBox.Show("Debe ingresar un titulo.", "Error", (MessageBoxButtons)MessageBoxButton.OK, (MessageBoxIcon)MessageBoxImage.Error);
                    txtTitulo.Focus();
                }
                else if (txtDescripcion.Text.Equals(""))
                {
                    MessageBox.Show("Debe ingresar una descripcion.", "Error", (MessageBoxButtons)MessageBoxButton.OK, (MessageBoxIcon)MessageBoxImage.Error);
                    txtDescripcion.Focus();
                }
                else if (txtDescripcion.Text.Length > 50)
                {
                    MessageBox.Show("Descripcion muy larga, no debe superar los 50 caracteres.");
                }
                else
                {
                    double precio = Convert.ToDouble(txtPrecio.Text);
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@Titulo", txtTitulo.Text);
                    cmd.Parameters.AddWithValue("@Descripcion", txtDescripcion.Text);
                    cmd.Parameters.AddWithValue("@Precio", precio);
                    cmd.Parameters.AddWithValue("@Foto", fotoSubidaPath);
                    cmd.ExecuteNonQuery();
                    connection.Close();

                    if (seleccionarFoto)
                    {
                        if (!Directory.Exists(destinationPath))
                        {
                            Directory.CreateDirectory(destinationPath);
                        }
                        File.Copy(fotoPath, fotoSubidaPath, true);
                    }
                    Home.GetListaProductos();
                    Close();
                }
            }
            catch
            {
                MessageBox.Show("Debe ingresar un precio correcto.", "Error", (MessageBoxButtons)MessageBoxButton.OK, (MessageBoxIcon)MessageBoxImage.Error);
            }
        }

        public static void Borrar(int id_producto)
        {
            string query = $"DELETE FROM Productos WHERE Id_Producto = {id_producto}";
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand(query, connection);
            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();
            Home.GetListaPrestamos();
        }
        private void Borrar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult decision = (MessageBoxResult)MessageBox.Show("¿Seguro que desea borrar?, todos los datos relacionados serán eliminados", "Confirmación de Borrado", (MessageBoxButtons)MessageBoxButton.YesNo, (MessageBoxIcon)MessageBoxImage.Warning);
            if (decision == MessageBoxResult.Yes)
            {
                Borrar(articulo.Id);
                Close();
            }
            else
            {

            }
      
        }
    }
}
