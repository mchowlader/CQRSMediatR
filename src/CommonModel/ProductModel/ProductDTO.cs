using System.ComponentModel.DataAnnotations;

namespace CommonModel.ProductModel
{
    public record ProductDTO
    {
        public int? Id { get; set; }

        [Required(ErrorMessage ="Product name is required.")]
        public string Name { get; set; } = string.Empty;
        
        [Required(ErrorMessage ="Price is required.")]
        [Range(0, double.MaxValue, ErrorMessage ="Price must be a positive number.")]
        public decimal Price { get; set; }
    }

    public record ProductResponse
    {
        public List<ProductDTO> Items { get; set; }
        public int TotalCount { get; set; }
    }
}
