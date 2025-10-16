namespace Proyecto_final_poo.Domain
{
    public class Producto : IIdentificable, INombrable
    {
        // id unico del producto
        public int Id { get; set; }

        // nombre del producto
        public string Nombre { get; set; } = "";

        // precio del producto
        public decimal Precio { get; set; }

        // stock disponible del producto
        public int Stock { get; set; }

        // id de la categoria a la que pertenece el producto
        public int CategoriaId { get; set; }
    }
}