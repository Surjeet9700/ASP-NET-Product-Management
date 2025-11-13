using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyAspNetCoreApp.Models
{
    public class ExternalApiLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string ApiName { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string RequestUrl { get; set; } = string.Empty;

        [StringLength(50)]
        public string? RequestMethod { get; set; }

        public string? RequestBody { get; set; }

        public string? ResponseBody { get; set; }

        public int? StatusCode { get; set; }

        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

        public bool IsSuccessful { get; set; }

        [StringLength(1000)]
        public string? ErrorMessage { get; set; }

        public int ResponseTimeMs { get; set; }
    }
}
