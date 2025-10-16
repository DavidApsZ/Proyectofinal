namespace Proyecto_final_poo.Domain
{
    public class Usuario : IIdentificable, INombrable
    {
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public string Nombre { get; set; } = "";
        public bool IsAdmin { get; set; }

    }  
}

