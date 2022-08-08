using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WhiteBeltCodeBlog.Models;

namespace WhiteBeltCodeBlog.Data
{
    public class ApplicationDbContext : IdentityDbContext<BlogUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}