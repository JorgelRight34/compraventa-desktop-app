using firstWPFApp.Models;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace firstWPFApp.View.UserControls
{
    /// <summary>
    /// Interaction logic for ListItemProducto.xaml
    /// </summary>
    public partial class ListItemProducto : UserControl
    {
        public Producto producto { get { return DataContext as Producto; } }
        public ListItemProducto()
        {
            InitializeComponent();
        }

        private void EditarArticulo_Click(object sender, RoutedEventArgs e)
        {
            AgregarArticulo agregarArticuloWindow = new AgregarArticulo(producto);
            agregarArticuloWindow.Show();
            Home.GetListaProductos();
        }

        private void Vender_Click(object sender, RoutedEventArgs e)
        {
            VenderArticulo venderArticuloWindow = new VenderArticulo(producto);
            venderArticuloWindow.Show();
        }
    }
}
