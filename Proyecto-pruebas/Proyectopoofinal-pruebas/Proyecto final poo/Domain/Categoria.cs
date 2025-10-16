namespace Proyecto_final_poo.Domain
{
    public class Categoria : IIdentificable, INombrable
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string? Descripcion { get; set; }

    }    
}
