using firstWPFApp.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Numerics;
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
    /// Interaction logic for EditarPrestamo.xaml
    /// </summary>
    public partial class EditarPrestamo : Window
    {
        public string connectionString = "Data Source=LAPTOP-BFV33PDI\\" +
                                        "SQLEXPRESS;Initial Catalog=CompraVentaApp;Integrated Security=True";
        public Prestamo prestamo;
        public string fotoPath;
        public string destinationPath;
        public string fotoSubidaPath;
        public EditarPrestamo(Prestamo prestamo)
        {
            InitializeComponent();

            this.prestamo = prestamo;
            //txtTasaInteres.Text = prestamo.TasaInteres;
            txtPrecio.Text = prestamo.Precio.ToString();
            txtArticuloTitulo.Text = prestamo.Titulo;
            FechaPicker.SelectedDate = prestamo.Fecha;
            ImagenArticulo.Source = new BitmapImage(new Uri(prestamo.Foto));
            txtDescripcion.Text = prestamo.Descripcion;
            PlazoComboBox.SelectedItem = prestamo.Plazo;
            fotoSubidaPath = prestamo.Foto;
        }
        private void SubirFoto_Click(object sender, RoutedEventArgs e)
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

                ImagenArticulo.Source = new BitmapImage(new Uri(fotoPath));
            }
            SubirFoto.Content = "";
        }

        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(fotoSubidaPath))
            {
                File.Copy(fotoPath, fotoSubidaPath, true);
            }

            Cliente? cliente = (Cliente)ClienteComboBox.SelectedItem;
            if (cliente == null)
            {
                MessageBox.Show("Debe seleccionar un cliente.");
                ClienteComboBox.Focus();
            }
            int? id_cliente = cliente.Id;

            if (PlazoComboBox.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar un plazo.");
                PlazoComboBox.Focus();
            }

            DateTime fecha = DateTime.Now;

            try
            {
                fecha = FechaPicker.SelectedDate.Value;
            }
            catch
            {
                MessageBox.Show("Debe seleccionar una fecha.");
            }

            if (txtDescripcion.Text == null)
            {
                MessageBox.Show("Debe ingresar una descripcion.");
            }
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

            if (!Directory.Exists(destinationPath))
            {
                Directory.CreateDirectory(destinationPath);
            }

            string plazo = ((ComboBoxItem)PlazoComboBox.SelectedItem).Content.ToString();
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            string query = $"UPDATE Prestamos " +
                           $"SET Precio = @Precio, Tasa_Interes = @Tasa_Interes, Plazo = @Plazo, Fecha = @Fecha" +
                           $"Titulo_Articulo = @Titulo_Articulo, Descripcion = @Descripcion," +
                           $" Foto = @Foto WHERE Id_Prestamo =  {prestamo.Id}";

            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Precio", txtPrecio.Text);
            cmd.Parameters.AddWithValue("@Tasa_Interes", txtTasaInteres.Text);
            cmd.Parameters.AddWithValue("@Plazo", plazo);
            cmd.Parameters.AddWithValue("@Fecha", fecha.ToString("MM/dd/yyyy"));
            cmd.Parameters.AddWithValue("@Titulo_Articulo", txtArticuloTitulo.Text);
            cmd.Parameters.AddWithValue("@Descripcion", txtDescripcion.Text);
            cmd.Parameters.AddWithValue("@Foto", fotoSubidaPath);
            cmd.ExecuteNonQuery();
            connection.Close();
            Close();
        }

        private void Borrar_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            string query = $"DELETE FROM Prestamos WHERE ID_Producto = {prestamo.Id}";
            MessageBox.Show(prestamo.Id.ToString());

            connection.Open();
            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.ExecuteNonQuery();
            connection.Close();

            Directory.Delete(fotoSubidaPath, false);

            Close();
        }
    }
}
