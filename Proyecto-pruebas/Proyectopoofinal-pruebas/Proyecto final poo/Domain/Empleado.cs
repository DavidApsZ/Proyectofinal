namespace Proyecto_final_poo.Domain
{
    public class Empleado : IIdentificable, INombrable
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string? Rol { get; set; }

    }  
}

