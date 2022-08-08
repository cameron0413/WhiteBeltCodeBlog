using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WhiteBeltCodeBlog.Models
{
    public class BlogUser : IdentityUser
    {
        [Required]
        [Display(Name = "First Name")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and a maximum of {1} characters.", MinimumLength = 2)]
        public string? FirstName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and a maximum of {1} characters.", MinimumLength = 2)]
        public string? LastName { get; set; }

        [NotMapped]
        public string? FullName { get { return $"{FirstName} {LastName}"; } }


        //Properties for storing image
        public byte[]? ImageData { get; set; }
        public string? ImageType { get; set; } = "";


        //Property for passing file information from the form(html) to the post.
        //Also not saved in the database via [NotMapped] attribute
        [NotMapped]
        public virtual IFormFile? ImageFile { get; set; }

        //Navigation Properties
    }
}
