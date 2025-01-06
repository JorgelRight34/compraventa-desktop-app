using firstWPFApp.View;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace firstWPFApp.Models
{
    public class Prestamo
    {
        public int Id { get; set; }
        public int Id_Cliente { get; set; }
        public string? Titulo { get; set; }
        public string? Descripcion { get; set; }
        
        public double TasaInteres { get; set; }
        public double Precio { get; set; }
        public string Foto { get; set; }
        public DateTime Fecha { get; set; }
        public string Plazo { get; set; }
        public string Estado { get; set; }
        public int? ProximoPago { get; set; }   

        public double Total_Pagado { get; set; }    
        public double CapitalPagado { get; set; }
        public double InteresPagado { get; set; }  
        public string NombreCliente { get; set; }
        public DateTime UltimoPagoFecha { get; set; }   

        public override string ToString()
        {
            return $"{Id}, {Titulo}, {Precio}";
        }
        public int DiasQueHanPasado(Prestamo prestamo, Pago pago)
        {
            return (pago.Fecha - prestamo.UltimoPagoFecha).Days;
        }

        public double Deuda()
        {
            string connectionString = "Data Source=LAPTOP-BFV33PDI\\" +
                                         "SQLEXPRESS;Initial Catalog=CompraVentaApp;" +
                                         "Integrated Security=True";
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            string query = $"SELECT * FROM Prestamos WHERE Id_Prestamo = {Id}";
            SqlCommand cmd = new SqlCommand(query, connection);
            SqlDataReader reader = cmd.ExecuteReader();
            double capitalPagado = 0;
            while (reader.Read())
            {
                capitalPagado = (double)reader["Capital_Pagado"];
            }
            double deuda = Precio - capitalPagado;
            return deuda;
        }

        public DateTime CalcularUltimoPago()
        {
            List<DateTime> fechaPagos = new List<DateTime>();
            foreach (Pago pago in Home.pagos)
            {
                if (pago.Id_Prestamo.Equals(Id))
                {
                    fechaPagos.Add(pago.Fecha);
                }
            }
            try
            {
                UltimoPagoFecha = fechaPagos.Max().Date;
                return fechaPagos.Max();
            }
            catch
            {
                UltimoPagoFecha = Fecha;
                return Fecha;
            }
        }
        public void CalcularFechaPago()
        {
            Dictionary<string, int> plazos = new Dictionary<string, int>
            {
                {"15 días", 15 }, {"1 mes", 31}, {"2 meses", 62}, {"3 meses", 93}
            };
            List<DateTime> fechaPagos = new List<DateTime>();

            foreach (Pago pago in Home.pagos)
            {
                if (pago.Id_Prestamo.Equals(Id))
                {
                    fechaPagos.Add(pago.Fecha);
                }
            }

            try
            {
                DateTime ultimaFecha = fechaPagos.Max();
                int plazo = plazos[Plazo];
                DateTime fechaPago = ultimaFecha.AddDays(plazo);
                TimeSpan diasRestantes = fechaPago - DateTime.Now;
                ProximoPago = (Convert.ToInt32(Math.Round(diasRestantes.TotalDays, 0)));
            }

            catch
            {
                if ((DateTime.Now - Fecha).TotalDays > 15)
                {
                    Estado = "Atrasado"; 
                }
                else
                {
                    ProximoPago = (int?)Math.Abs((decimal)(int?)((Fecha - DateTime.Now).TotalDays - plazos[Plazo]));
                }
            }
        }

        public static void Buscar(string query)
        {

            SqlConnection connection = Home.connection;
            connection.Open();
            SqlCommand cmd = new SqlCommand(query, connection);
            SqlDataReader reader = cmd.ExecuteReader();

            Home.prestamos.Clear();
            Home.prestamosPasados.Clear();
            while (reader.Read())
            {
                Prestamo prestamo = new Prestamo
                {
                    Id = (int)reader["Id_Prestamo"],
                    Id_Cliente = (int)reader["Id_Cliente"],
                    Titulo = (string)reader["Titulo_Articulo"],
                    Descripcion = (string)reader["Descripcion"],
                    Precio = (double)reader["Precio"],
                    Foto = (string)reader["Foto"],
                    TasaInteres = (double)reader["Tasa_Interés"],
                    Fecha = DateTime.Parse((string)reader["Fecha"]),
                    Plazo = (string)reader["Plazo"],
                    Estado = (string)reader["Estado"],
                };
                prestamo.CalcularFechaPago();
                if (prestamo.Estado.Equals("Saldado"))
                {
                    Home.prestamosPasados.Add(prestamo);
                }
                else
                {
                    Home.prestamos.Add(prestamo);
                }
            }
            connection.Close();
        }

        public static Prestamo GetPrestamo(int id_Prestamo)
        {
            string connectionString = "Data Source=LAPTOP-BFV33PDI\\" +
                                          "SQLEXPRESS;Initial Catalog=CompraVentaApp;" +
                                          "Integrated Security=True";

            SqlConnection connection = new SqlConnection(connectionString);
            string query = $"SELECT * FROM Prestamos WHERE Id_Prestamo = {id_Prestamo}";

            connection.Open();
            SqlCommand cmd = new SqlCommand(query, connection);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Prestamo prestamo = new Prestamo
                {
                    Id = (int)reader["Id_Prestamo"],
                    Id_Cliente = (int)reader["Id_Cliente"],
                    Titulo = (string)reader["Titulo_Articulo"],
                    Descripcion = (string)reader["Descripcion"],
                    Precio = (double)reader["Precio"],
                    Foto = (string)reader["Foto"],
                    TasaInteres = (double)reader["Tasa_Interés"],
                    Fecha = DateTime.Parse((string)reader["Fecha"]),
                    Plazo = (string)reader["Plazo"],
                    Estado = (string)reader["Estado"],
                };
                return prestamo;
            }
            return null;
        }
    }
}
