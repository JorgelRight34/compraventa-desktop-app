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
using System.Text.RegularExpressions;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;

namespace firstWPFApp.View
{
    /// <summary>
    /// Interaction logic for RegistrarCliente.xaml
    /// </summary>
    public partial class RegistrarCliente : Window
    {
        private string connectionString = "Data Source=LAPTOP-BFV33PDI\\" +
                                         "SQLEXPRESS;Initial Catalog=CompraVentaApp;Integrated Security=True";
        private Cliente cliente;
        private string query;
        public RegistrarCliente()
        {
            InitializeComponent();

            fechaNacimiento.SelectedDate = new DateTime(1990, 01, 01);
            fechaNacimiento.DisplayDate = new DateTime(1990, 01, 01);
            RegistrarClienteGrid.Visibility = Visibility.Visible;
            query = "INSERT INTO Clientes (Nombre_Completo,Cédula,Género,Email,Fecha_Nacimiento,Teléfono) " +
                               "VALUES (@Nombre_Completo,@Cédula,@Género,@Email,@Fecha_Nacimiento,@Teléfono)";
        }
        public RegistrarCliente(Cliente cliente)
        {
            InitializeComponent();

            EditarClienteGrid.Visibility = Visibility.Visible;
            this.cliente = cliente;
            fechaNacimiento.SelectedDate = DateTime.Parse(cliente.FechaNacimiento);
            fechaNacimiento.DisplayDate = DateTime.Parse(cliente.FechaNacimiento);
            txtNombreCompleto.Text = cliente.Nombre;
            txtCedula.Text = cliente.Cedula;
            comboGenero.SelectedValue = cliente.Genero;
            txtCorreo.Text = cliente.Email;
            txtTelefono.Text = cliente.Telefono;
            //fechaNacimiento.SelectedDate = DateTime.Parse(cliente.FechaNacimiento);
            query = "UPDATE Clientes SET Nombre_Completo = @Nombre_Completo, " +
                    "Cédula = @Cédula, Género = @Género, Email = @Email, " +
                    $"Fecha_Nacimiento = @Fecha_Nacimiento, Teléfono = @Teléfono WHERE Id_Cliente = {cliente.Id}";

        }
        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string querrySelect = $"SELECT * FROM Clientes WHERE Cédula = '{txtCedula.Text}'";
            SqlDataAdapter sda = new SqlDataAdapter(querrySelect, connection);
            DataTable dtable = new DataTable();
            sda.Fill(dtable);
            int cedula;

            DateTime fecha = DateTime.Now;
            try
            {
                fecha = fechaNacimiento.SelectedDate.Value;

                if (txtNombreCompleto.Equals(""))
                {
                    MessageBox.Show("Debe ingresar un nombre.", "Error", (MessageBoxButtons)MessageBoxButton.OK, (MessageBoxIcon)MessageBoxImage.Error);
                    txtNombreCompleto.Focus();
                }
                else if (!Regex.IsMatch(txtCedula.Text, @"^\d{3}-\d{7}-\d{1}$"))
                {
                    MessageBox.Show("Debe ingresar un cedula valida.", "Error", (MessageBoxButtons)MessageBoxButton.OK, (MessageBoxIcon)MessageBoxImage.Error);
                }
                else if (dtable.Rows.Count > 0 && RegistrarClienteGrid.Visibility == Visibility.Visible)
                {
                    MessageBox.Show("Esta cédula ya esta registrada");
                }
                else if (comboGenero.SelectedValue == null)
                {
                    MessageBox.Show("Debe escoger un genero.", "Error", (MessageBoxButtons)MessageBoxButton.OK, (MessageBoxIcon)MessageBoxImage.Error);
                    comboGenero.Focus();
                }
                else if (txtCorreo.Equals(""))
                {
                    MessageBox.Show("Debe ingresar un correo electronico.", "Error", (MessageBoxButtons)MessageBoxButton.OK, (MessageBoxIcon)MessageBoxImage.Error);
                    txtCorreo.Focus();
                }
                else if (!(Regex.IsMatch(txtTelefono.Text, @"^\+d{2} \d{3}-\d{3}-\d{4}$") ||
                    Regex.IsMatch(txtTelefono.Text, @"^\+d{1} \d{3}-\d{3}-\d{4}$") ||
                    Regex.IsMatch(txtTelefono.Text, @"^\d{3}-\d{3}-\d{4}$")))
                {
                    MessageBox.Show("Ingrese el teléfono en el formáto correcto.");
                }
                else
                {
                    string genero = ((ComboBoxItem)comboGenero.SelectedItem).Content.ToString();

                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@Nombre_Completo", txtNombreCompleto.Text);
                    cmd.Parameters.AddWithValue("@Cédula", txtCedula.Text);
                    cmd.Parameters.AddWithValue("@Género", genero);
                    cmd.Parameters.AddWithValue("@Email", txtCorreo.Text);
                    cmd.Parameters.AddWithValue("@Fecha_Nacimiento", fecha.ToString("dd/MM/yyyy"));
                    cmd.Parameters.AddWithValue("@Teléfono", txtTelefono.Text);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Se ha registrado exitosamente");
                    Close();
                    connection.Close();
                    Home.GetListaClientes();
                }
            }
            catch
            {
                MessageBox.Show("Debe escoger una fecha de nacimiento", "Error", (MessageBoxButtons)MessageBoxButton.OK, (MessageBoxIcon)MessageBoxImage.Error);
                fechaNacimiento.Focus();
            }
        }

        private void Borrar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult decision = (MessageBoxResult)MessageBox.Show("¿Seguro que desea borrar?, todos los datos relacionados serán eliminados", "Confirmación de Borrado", (MessageBoxButtons)MessageBoxButton.YesNo, (MessageBoxIcon)MessageBoxImage.Warning);
            if (decision == MessageBoxResult.Yes)
            {
                query = $"DELETE FROM Clientes WHERE Id_Cliente = {cliente.Id}";
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                connection.Close();
                Home.GetListaClientes();
                Close();
            } else
            {

            }
        }
    }

}
