namespace Proyecto_final_poo.Domain
{
    public class Empleado : IIdentificable, INombrable
    {
        // id unico del empleado
        public int Id { get; set; }

        // nombre del empleado
        public string Nombre { get; set; } = "";

        // rol del empleado, puede ser nulo
        public string? Rol { get; set; }

    }
}