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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace firstWPFApp.View
{
    /// <summary>
    /// Interaction logic for EditarArticulo.xaml
    /// </summary>
    public partial class EditarArticulo : Window
    {
        private Producto producto;
        public string connectionString = "Data Source=LAPTOP-BFV33PDI\\" +
                                         "SQLEXPRESS;Initial Catalog=CompraVentaApp;Integrated Security=True";
        public string fotoSubidaPath;
        public string fotoPath;
        public string destinationPath;

        public EditarArticulo(Producto producto)
        {
            InitializeComponent();

            this.producto = producto;
            txtTitulo.Text = producto.Titulo;
            txtDescripcion.Text = producto.Descripcion;
            txtPrecio.Text = producto.Precio.ToString();
            fotoSubidaPath = producto.Foto;
            imagenArticulo.Source = new BitmapImage(new Uri(producto.Foto));
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
                destinationPath = System.IO.Path.Combine(directorioActual, "Imagenes");
                fotoSubidaPath = System.IO.Path.Combine(destinationPath, fileName);

                // Display the selected image in the Image control
                imagenArticulo.Source = new BitmapImage(new Uri(fotoPath));
                // Save the image to the database
            }
            SubirFoto.Content = "";
        }

        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(fotoSubidaPath))
            {
                File.Copy(fotoPath, fotoSubidaPath, true);
            }

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            string query = $"UPDATE Productos " +
                           $"SET Titulo = @Titulo, Descripcion = @Descripcion, Precio = @Precio, Foto = @Foto " +
                           $"WHERE ID_Producto =  {producto.Id}";
            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Titulo", txtTitulo.Text);
            cmd.Parameters.AddWithValue("@Descripcion", txtDescripcion.Text);
            cmd.Parameters.AddWithValue("@Precio", txtPrecio.Text);
            cmd.Parameters.AddWithValue("@Foto", fotoSubidaPath);
            cmd.ExecuteNonQuery();
            connection.Close();
            Close();
        }

        private void Borrar_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            string query = $"DELETE FROM Productos WHERE ID_Producto = {producto.Id}";
            MessageBox.Show(producto.Id.ToString());

            connection.Open();
            SqlCommand cmd = new SqlCommand(query,connection);
            cmd.ExecuteNonQuery();
            connection.Close();
            Directory.Delete(fotoSubidaPath, false);

            Close();
        }
    }
}
