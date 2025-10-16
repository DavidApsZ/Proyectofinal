using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Proyecto_final_poo.Domain
{
    public class Cliente : IIdentificable, INombrable
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public List<Venta> HistorialCompras { get; set; } = new();

        public bool EmailValido()
        {
            if (string.IsNullOrWhiteSpace(Email)) return false;
            return Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

    }
}