namespace Proyecto_final_poo.Domain
{
    public interface IGeneraRecibo
    {
        string GenerarRecibo(decimal montoCobrado);
    }

    public abstract class Pago : IGeneraRecibo
    {
        public decimal MontoProcesado { get; protected set; }
        public abstract decimal Procesar(decimal total);
        public virtual string GenerarRecibo(decimal montoCobrado)
            => $"Pago por ${montoCobrado:0.00}";
    }

    public class PagoEfectivo : Pago
    {
        public override decimal Procesar(decimal total)
        {
            MontoProcesado = total;
            return MontoProcesado;
        }

        public override string GenerarRecibo(decimal montoCobrado)
            => $"Pago en EFECTIVO: ${montoCobrado:0.00}";
    }

    public class PagoTarjeta : Pago
    {
        public decimal ComisionPorc { get; set; } = 0.035m;
        public override decimal Procesar(decimal total)
        {
            var com = total * ComisionPorc;
            MontoProcesado = total + com;
            return MontoProcesado;
        }

        public override string GenerarRecibo(decimal montoCobrado)
            => $"Pago con TARJETA: ${montoCobrado:0.00} (incluye comisión)";
    }
}