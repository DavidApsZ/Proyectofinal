namespace Proyecto_final_poo.Domain
{
    public static class Sesion
    {
        public static Usuario? UsuarioActual { get; set; }
        public static bool EstaLogueado => UsuarioActual != null;
    }
}
