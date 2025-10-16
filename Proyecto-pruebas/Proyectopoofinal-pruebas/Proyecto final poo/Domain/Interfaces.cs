namespace Proyecto_final_poo.Domain
{
    // interfaz para objetos que tienen un id unico
    public interface IIdentificable
    {
        int Id { get; }
    }

    // interfaz para objetos que tienen nombre
    public interface INombrable
    {
        string Nombre { get; set; }
    }
}