namespace ElasticAppDemo.Host.Models
{
    public class Product:ElasticModelBase
    {
        public string Ean { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Brand { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public string Status { get; set; }
        public DateTime ReleaseDate { get; set; }
    }
}
