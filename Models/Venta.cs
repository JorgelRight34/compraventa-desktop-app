using firstWPFApp.View;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace firstWPFApp.Models
{
    public class Venta
    {
        public int Id { get; set; }
        public int Id_Producto { get; set; }
        public int Id_Cliente { get; set; }
        public double Monto { get; set; }
        public DateTime Fecha { get; set; }

        public string Foto { get; set; }    
        public string Descripcion { get; set; }

        public static DataTable Buscar(string busqueda, int id_Cliente)
        {
            SqlConnection connection = Home.connection;
            string query = "SELECT * FROM Ventas WHERE CONCAT([Id_Venta], [Id_Producto], [Id_Cliente], [Monto], " +
                            "[Fecha]) LIKE '%" + busqueda + "%'" + $" WHERE {id_Cliente}";

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
            string query = "SELECT * FROM Ventas WHERE CONCAT([Id_Venta], [Id_Producto], [Id_Cliente], [Monto], " +
                              "[Fecha]) LIKE '%" + busqueda + "%'";

            connection.Open();
            SqlCommand cmd = new SqlCommand(query, connection);
            SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            connection.Close();
            return dataTable;
        }



    }

}
