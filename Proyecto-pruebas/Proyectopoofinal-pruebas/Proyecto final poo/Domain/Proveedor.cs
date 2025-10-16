namespace Proyecto_final_poo.Domain
{
    public class Proveedor : IIdentificable, INombrable
    {
        // id unico del proveedor
        public int Id { get; set; }

        // nombre del proveedor
        public string Nombre { get; set; } = "";

        // telefono del proveedor, puede ser nulo
        public string? Telefono { get; set; }
    }
}