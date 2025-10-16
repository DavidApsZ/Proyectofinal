namespace Proyecto_final_poo.Domain
{
    public static class Sesion
    {
        // guarda el usuario actual que inicio sesion, puede ser nulo si nadie ha iniciado sesion
        public static Usuario? UsuarioActual { get; set; }

        // indica si hay un usuario logueado, true si usuarioactual no es nulo
        public static bool EstaLogueado => UsuarioActual != null;
    }
}