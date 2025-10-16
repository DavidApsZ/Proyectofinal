namespace Proyecto_final_poo.Domain
{
    // interfaz para clases que pueden generar un recibo
    public interface IGeneraRecibo
    {
        string GenerarRecibo(decimal montoCobrado);
    }

    // clase abstracta para pagos, implementa la interfaz de generar recibo
    public abstract class Pago : IGeneraRecibo
    {
        // guarda el monto que fue procesado en el pago
        public decimal MontoProcesado { get; protected set; }

        // metodo abstracto para procesar el pago, lo implementan las subclases
        public abstract decimal Procesar(decimal total);

        // metodo virtual para generar un recibo simple
        public virtual string GenerarRecibo(decimal montoCobrado)
            => $"Pago por ${montoCobrado:0.00}";
    }

    // clase para pagos en efectivo
    public class PagoEfectivo : Pago
    {
        // procesa el pago en efectivo, asigna el total al monto procesado
        public override decimal Procesar(decimal total)
        {
            MontoProcesado = total;
            return MontoProcesado;
        }

        // genera el recibo para pago en efectivo
        public override string GenerarRecibo(decimal montoCobrado)
            => $"Pago en EFECTIVO: ${montoCobrado:0.00}";
    }

    // clase para pagos con tarjeta
    public class PagoTarjeta : Pago
    {
        // porcentaje de comision por pago con tarjeta
        public decimal ComisionPorc { get; set; } = 0.035m;

        // procesa el pago con tarjeta, suma la comision al total
        public override decimal Procesar(decimal total)
        {
            var com = total * ComisionPorc;
            MontoProcesado = total + com;
            return MontoProcesado;
        }

        // genera el recibo para pago con tarjeta, indica que incluye comision
        public override string GenerarRecibo(decimal montoCobrado)
            => $"Pago con TARJETA: ${montoCobrado:0.00} (incluye comision)";
    }
}