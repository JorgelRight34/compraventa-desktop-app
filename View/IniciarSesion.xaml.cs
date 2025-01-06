using firstWPFApp.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
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
    /// Interaction logic for IniciarSesion.xaml
    /// </summary>
    public partial class IniciarSesion : Window
    {
        public string connectionString = "Data Source=LAPTOP-BFV33PDI\\" +
                                       "SQLEXPRESS;Initial Catalog=CompraVentaApp;Integrated Security=True";
        public IniciarSesion()
        {
            InitializeComponent();

        }
        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            if (txtEmail.Text.Equals(""))
            {
                MessageBox.Show("Debe ingresar un nombre.");
            }
            else if (txtContrasena.Password.Equals(""))
            {
                MessageBox.Show("Debe ingresar un Email.");
            }
            else
            {
                string query = $"SELECT * FROM Usuarios WHERE Email = '{txtEmail.Text}' " +
                               $"AND Contrasena = '{txtContrasena.Password}'";
                SqlDataAdapter sda = new SqlDataAdapter(query, connection);
                DataTable dtable = new DataTable();
                sda.Fill(dtable);

                if (dtable.Rows.Count > 0)
                {
                    Usuario empleado = logInUsuario(txtEmail.Text, txtContrasena.Password, connection);
                    Home homeWindow = new Home(empleado);
                    connection.Close();
                    Close();
                    homeWindow.Show();
                }
                else
                {
                    MessageBox.Show("Una cuenta con esos datos no existe");
                    txtEmail.Clear();
                    txtContrasena.Clear();
                }
            }
            connection.Close();
        }
        public static Usuario logInUsuario(string email, string contrasena, SqlConnection connection)
        {

            string query = $"SELECT * FROM Usuarios WHERE Email = '{email}' AND Contrasena = '{contrasena}'";
            SqlCommand cmd = new SqlCommand(query, connection);
            SqlDataReader reader = cmd.ExecuteReader();

            Usuario usuario = new Usuario();
            while (reader.Read())
            {
                usuario = new Usuario
                {
                    Id = (int)reader["Id_Usuario"],
                    Nombre = (string)reader["Nombre_Completo"],
                    Email = (string)reader["Email"],
                    Contrasena = (string)reader["Contrasena"],
                    Fecha_Nacimiento = DateTime.Parse((string)reader["Fecha_Nacimiento"]),
                };
            }
            return usuario;
        }
    

        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            Sign_Up sign_UpWindow = new Sign_Up();  
            sign_UpWindow.Show();   
        }

        private void textEmail_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtEmail.Focus();
        }

        private void txtEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtEmail.Text) && txtEmail.Text.Length > 0) 
            {
                textEmail.Visibility = Visibility.Collapsed;
            } 
            else
            {
                textEmail.Visibility = Visibility.Visible;
            }
        }

        private void textContrasena_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtContrasena.Focus();
        }

        private void txtContrasena_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtContrasena.Password) && txtContrasena.Password.Length > 0)
            {
                textContrasena.Visibility = Visibility.Collapsed;
            }
            else
            {
                textContrasena.Visibility = Visibility.Visible;
            }

        }
    }
}
