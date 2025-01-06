using firstWPFApp.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics.Eventing.Reader;
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

namespace firstWPFApp.View
{
    /// <summary>
    /// Interaction logic for RegistrarPago.xaml
    /// </summary>
    public partial class RegistrarPago : Window
    {
        private static string connectionString = "Data Source=LAPTOP-BFV33PDI\\SQLEXPRESS;" +
                                          "Initial Catalog=CompraVentaApp;Integrated Security=True";
        private string query2;
        private int Id_Prestamo;
        public Pago? pago;
        public Prestamo? prestamo;
        private List<Prestamo> prestamos = new List<Prestamo>();
        private SqlConnection connection = new SqlConnection(connectionString);
        public RegistrarPago(Prestamo prestamo)
        {
            InitializeComponent();
            this.prestamo = prestamo;
            Id_Prestamo = prestamo.Id;
            string[] fecha = prestamo.Fecha.ToString("MM/dd/yyyy").Split("/");
            int[] fechaSplit = Array.ConvertAll(fecha, int.Parse);
            fechaPago.DisplayDateStart = new DateTime(fechaSplit[2], fechaSplit[0], fechaSplit[1]);
            Guardar.Visibility = Visibility.Visible;
            editarPagoGrid.Visibility = Visibility.Hidden;
            query2 = "INSERT INTO Pagos (Id_Prestamo,Monto,Fecha_Pago,Capital_Pagado,Interés_Pagado) " +
                     "VALUES (@Id_Prestamo,@Monto,@Fecha_Pago,@Capital_Pagado,@Interés_Pagado)";
        }

        public RegistrarPago(Pago pago)
        {
            InitializeComponent();
            this.pago = pago;
            
            titulo.Text = "Editar Pago";
            txtMonto.Text = pago.Monto.ToString();
            Id_Prestamo = pago.Id_Prestamo;
            fechaPago.DisplayDate = pago.Fecha;
            fechaPago.SelectedDate = pago.Fecha;
            Guardar.Visibility = Visibility.Hidden;
            editarPagoGrid.Visibility = Visibility.Visible;
            query2 = $"UPDATE Pagos SET Id_Prestamo = @Id_Prestamo, Monto = @Monto, Fecha_Pago = @Fecha_Pago, " +
                    $"Capital Pagado = @Capital_Pagado, Interés_Pagado = @Interés_Pagado WHERE Id_Pago = {pago.Id}";
        }

        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            double monto = 0;
            DateTime fecha = DateTime.Now;
            bool guardar = true;
            string query = null;

            

            try
            {
                monto = Convert.ToDouble(txtMonto.Text);
            } 
            catch
            {
                MessageBox.Show("Debe ingresar un monto correcto.");
                guardar = false;
            }

            try
            {
                fecha = fechaPago.SelectedDate.Value;
            }
            catch
            {
                MessageBox.Show("Debe seleccionar una fecha");
                guardar = false;
            }
            Prestamo prestamoPrueba = Prestamo.GetPrestamo(Id_Prestamo);
            Pago pago = new Pago()
            {
                Id = 0,
                Id_Prestamo = 0,
                Monto = monto,
                Fecha = DateTime.Parse(fecha.ToString("MM/dd/yyyy")),
            };

            if (prestamoPrueba.CapitalPagado + Pago.CalcularCapital(prestamo, pago) > prestamoPrueba.Precio)
            {
                MessageBox.Show("El capital total pagado no puede exceder el precio del empeño.");

            }

            else if (guardar)
            {
                connection.Open();
                double capital = Pago.CalcularCapital(prestamo, pago);
                double interes = Pago.CalcularInteres(prestamo, pago);


                query = "UPDATE Prestamos SET Total_Pagado = Total_Pagado + @Monto, " +
                        $"Capital_Pagado = Capital_Pagado + {capital}, " +
                        $"Interés_Pagado = Interés_Pagado + {interes} " +
                        $"WHERE Id_Prestamo = {Id_Prestamo}";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Monto", monto);

               try
               {
                    cmd.ExecuteNonQuery();
                    cmd = new SqlCommand(query2, connection);
                    cmd.Parameters.AddWithValue("@Id_Prestamo", Id_Prestamo);
                    cmd.Parameters.AddWithValue("@Monto", monto);
                    cmd.Parameters.AddWithValue("@Fecha_Pago", fecha.ToString("MM/dd/yyyy"));
                    cmd.Parameters.AddWithValue("@Capital_Pagado", capital);
                    cmd.Parameters.AddWithValue("@Interés_Pagado", interes);
                    cmd.ExecuteNonQuery();
                    connection.Close();
                    Home.GetListaPagos();
                    Home.GetListaPrestamos();
                    Close();
              }
              catch
              {
                    MessageBox.Show("El capital total del prestamo excede el capital acordado.");
              }
              try
              {
                    InfoCliente.GetPagos();
              }
               catch { };

            }
        }

        private void Borrar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult decision = (MessageBoxResult)MessageBox.Show("¿Seguro que desea borrar?, todos los datos relacionados serán eliminados", "Confirmación de Borrado", (MessageBoxButtons)MessageBoxButton.YesNo, (MessageBoxIcon)MessageBoxImage.Warning);
            if (decision == MessageBoxResult.Yes)
            {
                query2 = "DELETE FROM Pagos WHERE Id_Pago = @Id_Pago";
                connection.Open();
                SqlCommand cmd = new SqlCommand(query2, connection);
                cmd.Parameters.AddWithValue("@Id_Pago", pago.Id);
                cmd.ExecuteNonQuery();
                connection.Close();

                Home.GetListaPagos();

                query2 = "UPDATE Prestamos SET " +
                        $"Capital_Pagado = Capital_Pagado - {pago.Capital_Pagado}, " +
                        $"Interés_Pagado = Interés_Pagado - {pago.Interes_Pagado} " +
                        $"WHERE Id_Prestamo = @Id_Prestamo";
                connection.Open();
                cmd = new SqlCommand(query2, connection);
                cmd.Parameters.AddWithValue("Id_Prestamo", pago.Id_Prestamo);
                cmd.ExecuteNonQuery();
                    
                Close();
            }
            else
            {

            }
                
          
        }
    }
}
