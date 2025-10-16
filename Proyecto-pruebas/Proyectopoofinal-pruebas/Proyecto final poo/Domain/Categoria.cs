namespace Proyecto_final_poo.Domain
{
    public class Categoria : IIdentificable, INombrable
    {
        // identificador unico de la categoria
        public int Id { get; set; }

        // nombre de la categoria
        public string Nombre { get; set; } = "";

        // descripcion opcional de la categoria
        public string? Descripcion { get; set; }
    }
}