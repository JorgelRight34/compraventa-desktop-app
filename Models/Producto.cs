using firstWPFApp.View;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace firstWPFApp.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public string? Titulo { get; set; }
        public string? Descripcion { get; set; }
        public string Precio { get; set; }
        public string Foto { get; set; }

        public static void Buscar(string busqueda)
        {
            SqlConnection connection = Home.connection;
            string query = "SELECT * FROM Productos WHERE CONCAT([ID_Producto], [Titulo], [Descripcion], " +
                            "[Precio], [Foto]) " +
                            "LIKE '%" + busqueda + "%'";

            connection.Open();
            SqlCommand cmd = new SqlCommand(query, connection);
            SqlDataReader reader = cmd.ExecuteReader();

            Home.productos.Clear();
            while (reader.Read())
            {
                Producto producto = new Producto
                {
                    Id = (int)reader["ID_Producto"],
                    Titulo = (string)reader["Titulo"],
                    Descripcion = (string)reader["Descripcion"],
                    Precio = reader["Precio"].ToString(),
                    Foto = (string)reader["Foto"],

                };
                Home.productos.Add(producto);
            }
            connection.Close();
        }

        public static void Buscar(string busqueda, int Id_Cliente)
        {
            SqlConnection connection = Home.connection;
            string query = "SELECT * FROM Productos WHERE CONCAT([ID_Producto], [ID_Usuario], [Titulo], [Descripcion], " +
                            "[Precio], [Foto]) " +
                            "LIKE '%" + busqueda + "%'" + $" WHERE Id_Cliente = {Id_Cliente}";

            connection.Open();
            SqlCommand cmd = new SqlCommand(query, connection);
            SqlDataReader reader = cmd.ExecuteReader();

            Home.productos.Clear();
            while (reader.Read())
            {
                Producto producto = new Producto
                {
                    Id = (int)reader["ID_Producto"],
                    Titulo = (string)reader["Titulo"],
                    Descripcion = (string)reader["Descripcion"],
                    Precio = reader["Precio"].ToString(),
                    Foto = (string)reader["Foto"],

                };
                Home.productos.Add(producto);
            }
            connection.Close();
        }

    }


}
