using System.ComponentModel.DataAnnotations;

namespace PdfGenerator.WebApi.Models
{
    public class GeneratePdfRequest
    {
        [Required]
        public string DocumentName { get; set; }

        [Required]
        public byte[] Html { get; set; }
    }
}