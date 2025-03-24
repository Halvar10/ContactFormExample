using System.ComponentModel.DataAnnotations;

namespace ContactFormExample.Models
{
    public class ContactFormModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Text { get; set; }

        public List<IFormFile>? Attachments { get; set; }
    }
}
