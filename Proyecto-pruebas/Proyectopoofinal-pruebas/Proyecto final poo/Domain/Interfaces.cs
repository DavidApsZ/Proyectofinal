namespace Proyecto_final_poo.Domain
{
    public interface IIdentificable
    {
        int Id { get; }
    }

    public interface INombrable
    {
        string Nombre { get; set; }
    }
}
