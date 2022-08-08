using System.ComponentModel.DataAnnotations;

namespace WhiteBeltCodeBlog.Models
{
    public class Comment
    {
        // Primary Key
        public int Id { get; set; }

        //Foreign Key
        public int BlogPostId { get; set; }

        [Required]
        public string? AuthorId { get; set; }

        [DataType(DataType.Date)]
        public DateTime Created { get; set; }

        [DataType(DataType.Date)]
        public DateTime? LastUpdated { get; set; }

        public string? UpdateReason { get; set; }

        [Required]
        [StringLength(1000, ErrorMessage = "The {0} must be at least {2} and a maximum of {1} characters.", MinimumLength = 2)]
        public string? Body { get; set; }


        // Navigation Property
        // Navigation properties are things that allow us to navigate the database. They DO NOT help the user navigate the application.
        public virtual BlogPost? BlogPost { get; set; }

        public virtual BlogUser? Author { get; set; }


    }
}
