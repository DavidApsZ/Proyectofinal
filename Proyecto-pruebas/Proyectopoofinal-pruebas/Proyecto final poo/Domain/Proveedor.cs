namespace Proyecto_final_poo.Domain
{
    public class Proveedor : IIdentificable, INombrable
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string? Telefono { get; set; }
    }
}

