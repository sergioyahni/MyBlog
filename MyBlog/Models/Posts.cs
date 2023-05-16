using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace MyBlog.Models
{
    public class Posts
    {
        [Required]
        public Guid Id { get; set; }

        [Display(Name = "Author")]
        [Required]
        public string Author { get; set; }

        [Display(Name = "Date")]
        [Required]
        public string Date { get; set; }

        [Display(Name = "Title")]
        [Required]
        public string Title { get; set; }

        [Display(Name = "Image")]
        public string Image { get; set; }

        [Display(Name = "Image Credit")]
        public string? ImageCredit { get; set; }

        [NotMapped]
        [Display(Name = "Image to Upload")]
        public IFormFile? ImageFile { get; set; }

        [Display(Name = "Intro")]
        public string? Intro { get; set; }

        [Display(Name = "Article")]
        [Required]
        public string Article { get; set; }
        

        public Posts() { }
    }
}
