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
    public class Cliente
    {
        public int Id { get; set; }
        public string Nombre { get; set; }   

        public string Cedula { get; set; }  
        public string Email { get; set; }  

        public string FechaNacimiento { get; set; }

        public string Telefono { get; set; }    

        public string Genero { get; set; }    


        public Cliente()
        {

        }

        public override string ToString()
        {
            return $"{Nombre}, {Email}, {Telefono}, {Id}";
        }

        public static void Buscar(string busqueda)
        {
            SqlConnection connection = Home.connection;
            string query = "SELECT * FROM Clientes WHERE CONCAT([Id_Cliente], [Nombre_Completo], [Cédula], [Género], " +
                            "[Email], [Fecha_Nacimiento], [Teléfono]) " +
                            "LIKE '%" + busqueda + "%'";

            connection.Open();
            SqlCommand cmd = new SqlCommand(query, connection);
            SqlDataReader reader = cmd.ExecuteReader();

            Home.clientes.Clear();
            while (reader.Read())
            {
                Cliente cliente = new Cliente
                {
                    Id = (int)reader["Id_Cliente"],
                    Nombre = (string)reader["Nombre_Completo"],
                    Email = (string)reader["Email"],
                    Cedula = (string)reader["Cédula"],
                    Genero = (string)reader["Género"],
                    FechaNacimiento = (string)reader["Fecha_Nacimiento"],
                    Telefono = (string)reader["Teléfono"],

                };
                Home.clientes.Add(cliente);
            }
            connection.Close();
        }
       
        public static Cliente GetCliente(int id_Cliente)
        {
            string connectionString = "Data Source=LAPTOP-BFV33PDI\\" +
                                          "SQLEXPRESS;Initial Catalog=CompraVentaApp;" +
                                          "Integrated Security=True";

            SqlConnection connection = new SqlConnection(connectionString);
            string query = $"SELECT * FROM Clientes WHERE Id_Cliente = {id_Cliente}";

            connection.Open();
            SqlCommand cmd = new SqlCommand(query, connection);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Cliente cliente = new Cliente
                {
                    Id = (int)reader["Id_Cliente"],
                    Nombre = (string)reader["Nombre_Completo"],
                    Email = (string)reader["Email"],
                    Cedula = (string)reader["Cédula"],
                    Genero = (string)reader["Género"],
                    FechaNacimiento = (string)reader["Fecha_Nacimiento"],
                    Telefono = (string)reader["Teléfono"],
                };
                return cliente;
            }
            return null;
        }
    }
}
