using System;
using System.Collections.Generic;
using System.Linq;

namespace Proyecto_final_poo.Domain
{
    public class Venta
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;      
    }
}
