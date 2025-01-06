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
    /// Interaction logic for MenuBar1.xaml
    /// </summary>
    public partial class MenuBar1 : UserControl
    {
        private Cliente cliente { get { return DataContext as Cliente; } }
        public MenuBar1()
        {
            InitializeComponent();
        }

        private void InfoCliente_Click(object sender, RoutedEventArgs e)
        {
            InfoCliente infoClienteWindow = new InfoCliente(cliente);
            infoClienteWindow.Show();
        }
    }
}
