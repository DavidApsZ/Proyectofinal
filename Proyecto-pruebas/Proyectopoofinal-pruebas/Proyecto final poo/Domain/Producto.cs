namespace Proyecto_final_poo.Domain
{
    public class Producto : IIdentificable, INombrable
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public int CategoriaId { get; set; }

    } 
}
