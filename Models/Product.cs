using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyAspNetCoreApp.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, 999999.99, ErrorMessage = "Price must be between 0.01 and 999999.99")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [StringLength(50, ErrorMessage = "Category cannot exceed 50 characters")]
        public string Category { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Brand cannot exceed 50 characters")]
        public string? Brand { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be a positive number")]
        public int StockQuantity { get; set; }

        [Required(ErrorMessage = "SKU is required")]
        [StringLength(50, ErrorMessage = "SKU cannot exceed 50 characters")]
        public string SKU { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedDate { get; set; }

        [StringLength(500, ErrorMessage = "Image URL cannot exceed 500 characters")]
        [Url(ErrorMessage = "Invalid URL format")]
        public string? ImageUrl { get; set; }
    }
}
