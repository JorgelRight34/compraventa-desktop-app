using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml; // Only for EPPlus method
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Diagnostics;
using OfficeOpenXml.Drawing.Chart;

namespace firstWPFApp.Models
{

    public class GenerarRegistros
    {
        public static string connectionString = "Data Source=LAPTOP-BFV33PDI\\" +
                                          "SQLEXPRESS;Initial Catalog=CompraVentaApp;" +
                                          "Integrated Security=True";
        public static void GenerarRegistro(string path)
        {

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheetPagos = package.Workbook.Worksheets.Add("Pagos");

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var query = "SELECT Id_Pago, Id_Prestamo, Monto, Fecha_Pago FROM Pagos";
                    var cmd = new SqlCommand(query, connection);
                    var dataTable = new DataTable();
                    using (var dataAdapter = new SqlDataAdapter(cmd))
                    {
                        dataAdapter.Fill(dataTable);
                    }

                    worksheetPagos.Cells["A1"].Value = "Id_Pago";
                    worksheetPagos.Cells["B1"].Value = "Id_Prestamo";
                    worksheetPagos.Cells["C1"].Value = "Monto";
                    worksheetPagos.Cells["D1"].Value = "Capital_Pagado";
                    worksheetPagos.Cells["E1"].Value = "Interés_Pagado";
                    worksheetPagos.Cells["F1"].Value = "Fecha_Pago";

                    int rowIndex = 2; 
                    foreach (DataRow row in dataTable.Rows)
                    {
                        worksheetPagos.Cells[rowIndex, 1].Value = row["Id_Pago"];
                        worksheetPagos.Cells[rowIndex, 2].Value = row["Id_Prestamo"];
                        worksheetPagos.Cells[rowIndex, 3].Value = row["Monto"];
                        worksheetPagos.Cells[rowIndex, 4].Value = Pago.CalcularCapital(Prestamo.GetPrestamo((int)row["Id_Prestamo"]), Pago.GetPago((int)row["Id_Pago"]));
                        worksheetPagos.Cells[rowIndex, 5].Value = Pago.CalcularInteres(Prestamo.GetPrestamo((int)row["Id_Prestamo"]), Pago.GetPago((int)row["Id_Pago"]));
                        worksheetPagos.Cells[rowIndex, 6].Value = row["Fecha_Pago"];

                        rowIndex++;
                    }
                    var chartPagos = worksheetPagos.Drawings.AddChart("PagosChart", eChartType.Line);
                    chartPagos.SetPosition(0, 5, 9, 0);
                    chartPagos.SetSize(800, 400);
                    var seriesPagos = chartPagos.Series.Add(worksheetPagos.Cells["D2:D" + (dataTable.Rows.Count + 1)], worksheetPagos.Cells["F2:F" + (dataTable.Rows.Count + 1)]);

                }

                string fileName = $"Pagos_{DateTime.Now.ToString("MM-dd-yyyy HH.mm.ss")}.xlsx";
                string filePath = Path.Combine(path, fileName); 
                package.SaveAs(new FileInfo(filePath));

                var worksheetVentas = package.Workbook.Worksheets.Add("Ventas");

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var query = "SELECT Id_Venta, Id_Producto, Id_Cliente, Monto, Fecha FROM Ventas";
                    var cmd = new SqlCommand(query, connection);
                    var dataTable = new DataTable();
                    using (var dataAdapter = new SqlDataAdapter(cmd))
                    {
                        dataAdapter.Fill(dataTable);
                    }

                    worksheetVentas.Cells["A1"].Value = "Id_Producto";
                    worksheetVentas.Cells["B1"].Value = "Id_Cliente";
                    worksheetVentas.Cells["C1"].Value = "Monto";
                    worksheetVentas.Cells["G1"].Value = "Descripción";
                    worksheetVentas.Cells["F1"].Value = "Fecha";

                    int rowIndex = 2; 
                    foreach (DataRow row in dataTable.Rows)
                    {
                        worksheetVentas.Cells[rowIndex, 1].Value = row["Id_Producto"];
                        worksheetVentas.Cells[rowIndex, 2].Value = row["Id_Cliente"];
                        worksheetVentas.Cells[rowIndex, 3].Value = row["Monto"];
                        //worksheetVentas.Cells[rowIndex, 7].Value = row["Descripcion"];
                        worksheetVentas.Cells[rowIndex, 7].Value = row["Fecha"];

                        rowIndex++;
                    }
                    var chartVentas = worksheetVentas.Drawings.AddChart("VentasChart", eChartType.Line);
                    chartVentas.SetPosition(0, 5, 9, 0);
                    chartVentas.SetSize(800, 400);
                    var seriesVentas = chartVentas.Series.Add(worksheetVentas.Cells["C2:C" + (dataTable.Rows.Count + 1)], worksheetVentas.Cells["F2:F" + (dataTable.Rows.Count + 1)]);
                }

                fileName = $"Ventas_{DateTime.Now.ToString("MM-dd-yyyy HH.mm.ss")}.xlsx";
                filePath = Path.Combine(path, fileName);
                package.SaveAs(new FileInfo(filePath));

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var query = "SELECT * FROM Prestamos";
                    var cmd = new SqlCommand(query, connection);
                    var dataTable = new DataTable();
                    using (var dataAdapter = new SqlDataAdapter(cmd))
                    {
                        dataAdapter.Fill(dataTable);
                    }

                    worksheetVentas.Cells["A1"].Value = "Id_Préstamo";
                    worksheetVentas.Cells["B1"].Value = "Id_Cliente";
                    worksheetVentas.Cells["C1"].Value = "Precio";
                    worksheetVentas.Cells["F1"].Value = "Tasa_Interés";
                    worksheetVentas.Cells["G1"].Value = "Plazo";
                    worksheetVentas.Cells["H1"].Value = "Fecha";
                    worksheetVentas.Cells["I1"].Value = "Título_Articulo";
                    worksheetVentas.Cells["J1"].Value = "Foto";
                    worksheetVentas.Cells["H1"].Value = "Descripción";
                    worksheetVentas.Cells["J1"].Value = "Total_Pagado";
                    worksheetVentas.Cells["K1"].Value = "Estado";
                    worksheetVentas.Cells["L1"].Value = "Capital_Pagado";
                    worksheetVentas.Cells["M1"].Value = "Interés_Pagado";

                    int rowIndex = 2;
                    foreach (DataRow row in dataTable.Rows)
                    {
                        worksheetVentas.Cells[rowIndex, 1].Value = row["Id_Prestamo"];
                        worksheetVentas.Cells[rowIndex, 2].Value = row["Id_Cliente"];
                        worksheetVentas.Cells[rowIndex, 3].Value = row["Precio"];
                        worksheetVentas.Cells[rowIndex, 4].Value = row["Tasa_Interés"];
                        worksheetVentas.Cells[rowIndex, 5].Value = row["Plazo"];
                        worksheetVentas.Cells[rowIndex, 6].Value = row["Fecha"];
                        worksheetVentas.Cells[rowIndex, 7].Value = "Titulo_Articulo";
                        worksheetVentas.Cells[rowIndex, 8].Value = "Foto";
                        worksheetVentas.Cells[rowIndex, 9].Value = "Descripcion";
                        worksheetVentas.Cells[rowIndex, 10].Value = "Total_Pagado";
                        worksheetVentas.Cells[rowIndex, 11].Value = "Estado";
                        worksheetVentas.Cells[rowIndex, 12].Value = "Capital_Pagado";
                        worksheetVentas.Cells[rowIndex, 13].Value = "Interés_Pagado";

                        rowIndex++;
                    }
                    var chartVentas = worksheetVentas.Drawings.AddChart("PrestamosChart", eChartType.Line);
                    chartVentas.SetPosition(0, 5, 9, 0);
                    chartVentas.SetSize(800, 400);
                    var seriesVentas = chartVentas.Series.Add(worksheetVentas.Cells["M2:M" + (dataTable.Rows.Count + 1)], worksheetVentas.Cells["H2:H" + (dataTable.Rows.Count + 1)]);
                }
                fileName = $"Prestamos_{DateTime.Now.ToString("MM-dd-yyyy HH.mm.ss")}.xlsx";
                filePath = Path.Combine(path, fileName); 
                package.SaveAs(new FileInfo(filePath));

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var query = "SELECT * FROM Productos";
                    var cmd = new SqlCommand(query, connection);
                    var dataTable = new DataTable();
                    using (var dataAdapter = new SqlDataAdapter(cmd))
                    {
                        dataAdapter.Fill(dataTable);
                    }

                    worksheetVentas.Cells["A1"].Value = "Id_Producto";
                    worksheetVentas.Cells["B1"].Value = "Id_Cliente";
                    worksheetVentas.Cells["C1"].Value = "Título";
                    worksheetVentas.Cells["F1"].Value = "Descripción";
                    worksheetVentas.Cells["G1"].Value = "Precio";
                    worksheetVentas.Cells["H1"].Value = "Foto";
                    
                    int rowIndex = 2; 
                    foreach (DataRow row in dataTable.Rows)
                    {
                        worksheetVentas.Cells[rowIndex, 1].Value = row["Id_Producto"];
                        worksheetVentas.Cells[rowIndex, 2].Value = row["Id_Cliente"];
                        worksheetVentas.Cells[rowIndex, 3].Value = row["Titulo"];
                        worksheetVentas.Cells[rowIndex, 4].Value = row["Descripcion"];
                        worksheetVentas.Cells[rowIndex, 5].Value = row["Precio"];
                        worksheetVentas.Cells[rowIndex, 6].Value = row["Foto"];

                        rowIndex++;
                    }
                }
                fileName = $"Productos_{DateTime.Now.ToString("MM-dd-yyyy HH.mm.ss")}.xlsx";
                filePath = Path.Combine(path, fileName); 
                package.SaveAs(new FileInfo(filePath));

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var query = "SELECT * FROM Clientes";
                    var cmd = new SqlCommand(query, connection);
                    var dataTable = new DataTable();
                    using (var dataAdapter = new SqlDataAdapter(cmd))
                    {
                        dataAdapter.Fill(dataTable);
                    }

                    worksheetVentas.Cells["A1"].Value = "Id_Cliente";
                    worksheetVentas.Cells["B1"].Value = "Nombre_Completo";
                    worksheetVentas.Cells["C1"].Value = "Cédula";
                    worksheetVentas.Cells["F1"].Value = "Género";
                    worksheetVentas.Cells["G1"].Value = "Email";
                    worksheetVentas.Cells["H1"].Value = "Fecha_Nacimiento";
                    worksheetVentas.Cells["F1"].Value = "Teléfono";

                    int rowIndex = 2;
                    foreach (DataRow row in dataTable.Rows)
                    {
                        worksheetVentas.Cells[rowIndex, 1].Value = row["Id_Cliente"];
                        worksheetVentas.Cells[rowIndex, 2].Value = row["Nombre_Completo"];
                        worksheetVentas.Cells[rowIndex, 3].Value = row["Cédula"];
                        worksheetVentas.Cells[rowIndex, 4].Value = row["Género"];
                        worksheetVentas.Cells[rowIndex, 5].Value = row["Email"];
                        worksheetVentas.Cells[rowIndex, 6].Value = row["Fecha_Nacimiento"];
                        worksheetVentas.Cells[rowIndex, 7].Value = row["Teléfono"];

                        rowIndex++;
                    }
                }
                fileName = $"Clientes_{DateTime.Now.ToString("MM-dd-yyyy HH.mm.ss")}.xlsx";
                filePath = Path.Combine(path, fileName); 
                package.SaveAs(new FileInfo(filePath));

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var query = "SELECT * FROM Usuarios";
                    var cmd = new SqlCommand(query, connection);
                    var dataTable = new DataTable();
                    using (var dataAdapter = new SqlDataAdapter(cmd))
                    {
                        dataAdapter.Fill(dataTable);
                    }

                    worksheetVentas.Cells["A1"].Value = "Id_Usuario";
                    worksheetVentas.Cells["B1"].Value = "Nombre_Completo";
                    worksheetVentas.Cells["C1"].Value = "Cédula";
                    worksheetVentas.Cells["G1"].Value = "Email";
                    worksheetVentas.Cells["F1"].Value = "Teléfono";
                    worksheetVentas.Cells["H1"].Value = "Fecha_Nacimiento";

                    int rowIndex = 2;
                    foreach (DataRow row in dataTable.Rows)
                    {
                        worksheetVentas.Cells[rowIndex, 1].Value = row["Id_Usuario"];
                        worksheetVentas.Cells[rowIndex, 2].Value = row["Nombre_Completo"];
                        worksheetVentas.Cells[rowIndex, 3].Value = row["Cédula"];
                        worksheetVentas.Cells[rowIndex, 4].Value = row["Email"];
                        worksheetVentas.Cells[rowIndex, 7].Value = row["Teléfono"];
                        worksheetVentas.Cells[rowIndex, 6].Value = row["Fecha_Nacimiento"];

                        rowIndex++;
                    }
                }
                fileName = $"Usuarios_{DateTime.Now.ToString("MM-dd-yyyy HH.mm.ss")}.xlsx";
                filePath = Path.Combine(path, fileName); 
                package.SaveAs(new FileInfo(filePath));


            }


        }

    }
}
