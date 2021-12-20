namespace Tickets.Models.Enums
{
    public static class PaymentTypeTextEnum
    {
        public static readonly string Efectivo = "Pago en efectivo";
        public static readonly string TarjetaDeCredito = "Pago con tarjeta de Crédito";
        public static readonly string Cheques = "Pago por cheque";
        public static readonly string NotaDeCredito = "Nota de Crédito";
        public static readonly string DepTransf = "Pago por depósito/Transferencia";
        public static readonly string CashAdvance = "Pago por avance de efectivo";
    }
}