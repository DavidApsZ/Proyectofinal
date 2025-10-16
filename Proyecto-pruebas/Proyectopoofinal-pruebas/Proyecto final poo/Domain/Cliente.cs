using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Proyecto_final_poo.Domain
{
    public class Cliente : IIdentificable, INombrable
    {
        // id unico del cliente
        public int Id { get; set; }

        // nombre del cliente
        public string Nombre { get; set; } = "";

        // telefono del cliente, puede ser nulo
        public string? Telefono { get; set; }

        // email del cliente, puede ser nulo
        public string? Email { get; set; }

        // metodo para validar si el email es valido
        public bool EmailValido()
        {
            if (string.IsNullOrWhiteSpace(Email)) return false;
            return Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

    }
}