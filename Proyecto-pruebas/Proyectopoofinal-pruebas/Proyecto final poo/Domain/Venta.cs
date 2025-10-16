using System;

namespace Proyecto_final_poo.Domain
{
    public class Venta
    {
        // id unico de la venta
        public int Id { get; set; }

        // fecha en que se realizo la venta, por defecto es la fecha actual
        public DateTime Fecha { get; set; } = DateTime.Now;

        // id del cliente (puede ser 0 si fue venta al publico en general)
        public int ClienteId { get; set; }

        // nombre del cliente
        public string ClienteNombre { get; set; } = "";

        // total de la venta
        public decimal Total { get; set; }

        // id del empleado que atendió
        public int EmpleadoId { get; set; }

        // método de pago (Efectivo/Tarjeta)
        public string MetodoPago { get; set; } = "";
    }
}