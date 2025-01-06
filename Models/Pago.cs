using firstWPFApp.View;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace firstWPFApp.Models
{
    public class Pago
    {
        public int Id { get; set; }
        public int Id_Prestamo { get; set; }    
        public double Monto { get; set; }
        public DateTime Fecha { get; set; } 
        public double Capital_Pagado { get; set; }  
        public double Interes_Pagado { get; set; }  
        public Prestamo Prestamo() 
        {
            SqlConnection connection = Home.connection;
            string query = $"SELECT * FROM Prestamos WHERE Id_Prestamo = {Id_Prestamo}";
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
                    CapitalPagado = (double)reader["Capital_Pagado"],
                    InteresPagado = (double)reader["Interés_Pagado"],
                    NombreCliente = Cliente.GetCliente((int)reader["Id_Cliente"]).Nombre,
                };
                connection.Close();
                return prestamo;
            }
            connection.Close();
            return null;
        }

        public static DataTable Buscar(string busqueda, int Id_Cliente)
        {
            SqlConnection connection = Home.connection;
            string query = "SELECT * FROM Pagos WHERE CONCAT([Id_Pago], [Id_Prestamo], [Monto], [Fecha_Pago]) " +
                            "LIKE '%" + busqueda + "%'" + $" WHERE Id_Cliente = {Id_Cliente}";

            connection.Open();
            SqlCommand cmd = new SqlCommand(query, connection);
            SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            connection.Close();
            return dataTable;
        }
        public static DataTable Buscar(string busqueda)
        {
            SqlConnection connection = Home.connection;
            string query = "SELECT * FROM Pagos WHERE CONCAT([Id_Pago], [Id_Prestamo], [Monto], [Fecha_Pago]) " +
                            "LIKE '%" + busqueda + "%'";

            connection.Open();
            SqlCommand cmd = new SqlCommand(query, connection);
            SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            connection.Close();
            return dataTable;
        }

        public static double CalcularInteres(Prestamo prestamo, Pago pago)
        {
            Dictionary<string, int> plazos = new Dictionary<string, int>
            {
                {"15 días", 15 }, {"1 mes", 31}, {"2 meses", 62}, {"3 meses", 93}
            };

            double plazo = Convert.ToDouble(plazos[prestamo.Plazo]);
            double tasaInteres = prestamo.TasaInteres;
            double interesPorDia = (tasaInteres / 100) / plazo;
            double interes = Math.Round(prestamo.Deuda() * interesPorDia * prestamo.DiasQueHanPasado(prestamo, pago), 2);

            return interes;
        }

        public static double CalcularCapital(Prestamo prestamo, Pago pago)
        {
            Dictionary<string, int> plazos = new Dictionary<string, int>
            {
                {"15 días", 15 }, {"1 mes", 31}, {"2 meses", 62}, {"3 meses", 93}
            };

            double capitalPagado = pago.Monto - CalcularInteres(prestamo, pago);
            /*double plazo = Convert.ToDouble(plazos[prestamo.Plazo]);
            double tasaInteres = prestamo.TasaInteres;
            double interesPorDia = (prestamo.Deuda() * (tasaInteres / 100)) / plazo;
            */ //double capitalPagado = Math.Round(pago.Monto - (pago.Monto * interesPorDia) * prestamo.DiasQueHanPasado(prestamo, pago), 2);
            return capitalPagado;
        }

        public static Pago GetPago(int id_Pago)
        {
            string connectionString = "Data Source=LAPTOP-BFV33PDI\\" +
                                          "SQLEXPRESS;Initial Catalog=CompraVentaApp;" +
                                          "Integrated Security=True";

            SqlConnection connection = new SqlConnection(connectionString);
            string query = $"SELECT * FROM Pagos WHERE Id_Pago = {id_Pago}";

            connection.Open();
            SqlCommand cmd = new SqlCommand(query, connection);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Pago pago = new Pago
                {

                    Id = (int)reader["Id_Pago"],
                    Id_Prestamo = (int)reader["Id_Prestamo"],
                    Monto = (double)reader["Monto"],
                    Fecha = DateTime.Parse((string)reader["Fecha_Pago"]),
                };
                return pago;
            }
            return null;
        }
    }
}
