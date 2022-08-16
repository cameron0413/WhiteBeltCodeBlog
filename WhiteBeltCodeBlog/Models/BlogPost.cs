using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WhiteBeltCodeBlog.Enums;

namespace WhiteBeltCodeBlog.Models
{
    public class BlogPost
    {
        //Primary Key below:
        public int Id { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and a maximum of {1} characters.", MinimumLength = 2)]
        public string? Title { get; set; }

        [Required]
        public string? Content { get; set; }

        
        [DataType(DataType.Date)]
        public DateTime Created { get; set; }

        [DataType(DataType.Date)]
        public DateTime? LastUpdated { get; set; }

        //public BlogPostStatus BlogPostStatus { get; set; }

        [DisplayName("Published")]
        public bool IsPublished { get; set; }

        public string? Slug { get; set; }
        //The slug, aside from being a garden pest, is a mask we use for the user to make the URL look human-readable.

        public string? Abstract { get; set; }

        public bool IsDeleted { get; set; }

        //Properties for storing image
        public byte[]? ImageData { get; set; }
        public string? ImageType { get; set; } = "";


        //Property for passing file information from the form(html) to the post.
        //Also not saved in the database via [NotMapped] attribute
        [NotMapped]
        public virtual IFormFile? BlogPostImage { get; set; }


        // Navigation Properties
        public virtual Category? Category { get; set; }

        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
        public virtual ICollection<Tag> Tags { get; set; } = new HashSet<Tag>();


    }
}