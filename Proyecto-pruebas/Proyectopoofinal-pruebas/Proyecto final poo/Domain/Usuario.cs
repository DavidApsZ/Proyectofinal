namespace Proyecto_final_poo.Domain
{
    public class Usuario : IIdentificable, INombrable
    {
        // id unico del usuario
        public int Id { get; set; }

        // nombre de usuario para iniciar sesion
        public string Username { get; set; } = "";

        // nombre real del usuario
        public string Nombre { get; set; } = "";

        // indica si el usuario es administrador
        public bool IsAdmin { get; set; }

    }
}