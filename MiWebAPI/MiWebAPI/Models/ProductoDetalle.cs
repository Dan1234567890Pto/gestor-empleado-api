namespace MiWebAPI.Models
{
    public class ProductoDetalle
    {
        public long ID {  get; set; }
        public string? NombreProducto { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        public decimal Total { get; set; }
        public string? Descripcion { get; set; }
    }
}
