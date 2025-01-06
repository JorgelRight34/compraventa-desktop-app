using firstWPFApp.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for Sign_Up.xaml
    /// </summary>
    public partial class Sign_Up : Window
    {
        Cliente usuario;
        public Sign_Up()
        {
            InitializeComponent();
        }

        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = "Data Source=LAPTOP-BFV33PDI\\" +
                                      "SQLEXPRESS;Initial Catalog=CompraVentaApp;Integrated Security=True";
            SqlConnection connection = new SqlConnection(connectionString);
            bool guardar = true;
            connection.Open();
            

            string querrySelect = $"SELECT * FROM Usuarios WHERE Email = '{txtCorreo.Text}'";
            SqlDataAdapter sda = new SqlDataAdapter(querrySelect, connection);
            DataTable dtable = new DataTable();
            DateTime fecha = DateTime.Now;
            sda.Fill(dtable);
           
            if (dtable.Rows.Count > 0)
            {
                MessageBox.Show("Este correo ya esta registrado");
                guardar = false;
            }
            else if (txtNombreCompleto.Equals(""))
            {
                MessageBox.Show("Debe ingresar un nombre.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                txtNombreCompleto.Focus();
                guardar = false;
            }
            else if (!Regex.IsMatch(txtCedula.Text, @"^\d{3}-\d{7}-\d{1}$"))
            {
                MessageBox.Show("Debe ingresar un cedula valida.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                guardar = false;
            }
            else if (dtable.Rows.Count > 0)
            {
                MessageBox.Show("Esta cédula ya esta registrada");
                guardar = false;
            }
            else if (comboGenero.SelectedValue == null)
            {
                MessageBox.Show("Debe escoger un genero.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                comboGenero.Focus();
                guardar = false;
            }
            else if (txtCorreo.Equals(""))
            {
                MessageBox.Show("Debe ingresar un correo electronico.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                txtCorreo.Focus();
                guardar = false;
            }
            try
            {
                fecha = fechaNacimiento.SelectedDate.Value;
            }
            catch
            {
                MessageBox.Show("Debe escoger una fecha de nacimiento", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                fechaNacimiento.Focus();
                guardar = false;
            }
            if (guardar)
            {

            }
            if (!(Regex.IsMatch(txtTelefono.Text, @"^\+d{2} \d{3}-\d{3}-\d{4}$") ||
                Regex.IsMatch(txtTelefono.Text, @"^\+d{1} \d{3}-\d{3}-\d{4}$") ||
                Regex.IsMatch(txtTelefono.Text, @"^\d{3}-\d{3}-\d{4}$")) && guardar)
            {
                MessageBox.Show("Ingrese el teléfono en el formáto correcto.");
            }
            else if (txtContrasena.Text == "") 
            {
                MessageBox.Show("Debe ingresar una contrasena");
            }
            else
            {
                string query = "INSERT INTO Usuarios (Nombre_Completo,Email,Contrasena,Teléfono,Fecha_Nacimiento) " +
                               "VALUES (@Nombre_Completo,@Email,@Contrasena,@Teléfono,@Fecha_Nacimiento)";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Nombre_Completo", txtNombreCompleto.Text);
                cmd.Parameters.AddWithValue("@Cédula", txtCedula.Text);
                cmd.Parameters.AddWithValue("@Email", txtCorreo.Text);
                cmd.Parameters.AddWithValue("@Teléfono", txtTelefono.Text);
                cmd.Parameters.AddWithValue("@Contrasena", txtContrasena.Text);
                cmd.Parameters.AddWithValue("@Fecha_Nacimiento", fecha.ToString("MM/dd/yyyy"));
                cmd.ExecuteNonQuery();
                MessageBox.Show("Se ha registrado exitosamente");

                Usuario usuario = IniciarSesion.logInUsuario(txtCorreo.Text,txtContrasena.Text, connection);
                Home homeWindow = new Home(usuario);
                connection.Close();
                homeWindow.Show();
                Close();
            }
        }
    }
}
