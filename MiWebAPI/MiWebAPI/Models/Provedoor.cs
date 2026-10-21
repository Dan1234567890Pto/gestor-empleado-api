namespace MiWebAPI.Models
{
    public class Proveedor
    {
        public long Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string ApellidoPaterno { get; set; } = string.Empty;
        public string ApellidoMaterno { get; set; } = string.Empty;
        public string Rfc { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public decimal Credito { get; set; }
        public string NombreBanco { get; set; } = string.Empty;
    }
}