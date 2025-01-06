using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace firstWPFApp.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Contrasena { get; set; }
        public DateTime Fecha_Nacimiento { get; set; }
    }
}
