namespace MauiHybridApp.Models
{
    public record ProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }

    public record ProductResponse
    {
        public List<ProductDTO> Items { get; set; }
        public int TotalCount { get; set; }
    }
}
