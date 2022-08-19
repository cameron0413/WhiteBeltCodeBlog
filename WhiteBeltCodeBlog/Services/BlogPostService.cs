using WhiteBeltCodeBlog.Services.Interfaces;
using WhiteBeltCodeBlog.Services;
using WhiteBeltCodeBlog.Models;
using Microsoft.EntityFrameworkCore;
using WhiteBeltCodeBlog.Data;
using WhiteBeltCodeBlog.Extensions;

namespace WhiteBeltCodeBlog.Services
{
    public class BlogPostService : IBlogPostService
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageService _imageService;
        public BlogPostService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> ValidateSlugAsync(string title, int blogId)
        {
            try
            {
                string newSlug = title.Sluggify();

                if (blogId == 0)
                {
                    return !(await _context.BlogPosts.AnyAsync(b => b.Slug == newSlug));
                }
                else
                {
                    BlogPost blogPost = await _context.BlogPosts.AsNoTracking().FirstAsync(b => b.Id == blogId);

                    string oldSlug = blogPost.Slug!;

                    if (!string.Equals(oldSlug, newSlug))
                    {
                        return !(await _context.BlogPosts.AnyAsync(b => b.Slug == newSlug));
                    }
                }

                return true;


            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task AddTagToBlogPostAsync(int tagId, int blogPostId)
        {
            try
            {
                if (!await IsTagOnBlogPostAsync(tagId, blogPostId))
                {
                    BlogPost? blogPost = await _context.BlogPosts.FindAsync(blogPostId);
                    Tag? tag = await _context.Tags.FindAsync(tagId);

                    if (tag != null && blogPost != null)
                    {
                        blogPost.Tags.Add(tag);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task RemoveTagFromBlogPostAsync(int tagId, int blogPostId)
        {
            try
            {
                if (await IsTagOnBlogPostAsync(tagId, blogPostId))
                {
                    BlogPost? blogPost = await _context.BlogPosts.FindAsync(blogPostId);
                    Tag? tag = await _context.Tags.FindAsync(tagId);

                    if (tag != null && blogPost != null)
                    {
                        blogPost.Tags.Remove(tag);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> IsTagOnBlogPostAsync(int tagId, int blogPostId)
        {
            try
            {
                Tag? tag = await _context.Tags.FindAsync(tagId);
                return (await _context.BlogPosts.Include(c => c.Tags).FirstOrDefaultAsync(b => b.Id == blogPostId))!.Tags.Contains(tag!);
            }
            catch
            {

                throw;
            }
        }

        public async Task<IEnumerable<Tag>> GetBlogPostTagsAsync(int blogPostId)
        {

            BlogPost? blogPost = await _context.BlogPosts.Include(c => c.Tags).FirstOrDefaultAsync(c => c.Id == blogPostId);

            return blogPost.Tags.ToList();
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            List<Category>? categories = await _context.Categories.ToListAsync();

            return categories;


        }

        public async Task<List<BlogPost>> GetAllBlogPostsAsync()
        {
            List<BlogPost> blogPosts = await _context.BlogPosts
                                                 .Where(b => b.IsDeleted == false)
                                                 .Include(b => b.Comments)
                                                    .ThenInclude(b => b.Author)
                                                 .Include(b => b.Category)
                                                 .Include(b => b.Tags)
                                                 .ToListAsync();

            return blogPosts;
        }

        public async Task<List<BlogPost>> GetPopularBlogPostsAsync(int count)
        {
            List<BlogPost> blogPosts = await _context.BlogPosts
                                                     .OrderByDescending(b => b.Comments.Count)
                                                     .Take(count)
                                                     .ToListAsync();


            return blogPosts;
        }

        public async Task<List<BlogPost>> GetRecentBlogPostsAsync(int count)
        {
            List<BlogPost> blogPosts = await _context.BlogPosts
                                                     .OrderByDescending(b => b.Created)
                                                     .Take(count)
                                                     .ToListAsync();



            return blogPosts;
        }

        public IEnumerable<BlogPost> Search(string searchString)
        {
            try
            {
                IEnumerable<BlogPost> blogPosts = new List<BlogPost>();

                if (string.IsNullOrWhiteSpace(searchString))
                {
                    return blogPosts;
                }
                else
                {
                    searchString = searchString.Trim().ToLower();

                    blogPosts = _context.BlogPosts.Where(b => b.Title!.ToLower().Contains(searchString) ||
                                                              b.Abstract!.ToLower().Contains(searchString) ||
                                                              b.Content!.ToLower().Contains(searchString) ||
                                                              b.Category!.Name!.ToLower().Contains(searchString) ||
                                                              b.Comments.Any(
                                                                  c => c.Body!.ToLower().Contains(searchString) ||
                                                                      c.Author!.FirstName!.ToLower().Contains(searchString) ||
                                                                      c.Author!.LastName!.ToLower().Contains(searchString)) ||
                                                              b.Tags.Any(
                                                                  t => t.Name!.ToLower().Contains(searchString)))
                                                  .Include(b => b.Comments)
                                                    .ThenInclude(c => c.Author)
                                                  .Include(b => b.Category)
                                                  .Include(b => b.Tags)
                                                  .Where(b => b.IsDeleted == false && b.IsPublished == true)
                                                  .AsNoTracking()
                                                  .OrderByDescending(b=>b.Created)
                                                  .AsEnumerable();

                    return blogPosts;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<BlogPost>> GetBlogPostsInCategoryAsync(int categoryId, int count)
        {
            try
            {
                List<BlogPost> blogPosts = await _context.BlogPosts
                                                 .Where(b => b.IsDeleted == false && b.CategoryId == categoryId)
                                                 .Include(b => b.Comments)
                                                    .ThenInclude(b => b.Author)
                                                 .Include(b => b.Category)
                                                 .Include(b => b.Tags)
                                                 .OrderByDescending(b => b.Created)
                                                 .Take(count)
                                                 .ToListAsync();

                return blogPosts;
            }
            catch
            {

                throw;
            }
        }

        public async Task<List<BlogPost>> GetBlogPostsWithTagAsync(int tagId, int count)
        {
            try
            {
                List<BlogPost> blogPosts = await _context.BlogPosts
                                                 .Where(b => b.IsDeleted == false && b.Tags.Any(t => t.Id == tagId))
                                                 .Include(b => b.Comments)
                                                    .ThenInclude(b => b.Author)
                                                 .Include(b => b.Category)
                                                 .Include(b => b.Tags)
                                                 .OrderByDescending(b => b.Created)
                                                 .Take(count)
                                                 .ToListAsync();

                return blogPosts;
            }
            catch
            {

                throw;
            }
        }



        ///////////////////////////         Below are the commented-out methods that return lists               //////////////////////////////





        //public async Task<BlogPost> GetPopularBlogPostsAsync(int count)
        //{
        //    List<BlogPost> mostPopular = await _context.BlogPosts.OrderByDescending(b => b.Comments)
        //                                                       .Take(count)
        //                                                       .ToListAsync();


        //    return blogPosts;
        //}

        //// Also write theae as functions that return lists
        //public async Task<BlogPost> GetRecentBlogPostsAsync(int count)
        //{
        //    List<BlogPost> blogPosts = await _context.BlogPosts
        //                                             .OrderByDescending(b => b.Created)
        //                                             .Take(count)
        //                                             .ToListAsync();

        //    return blogPosts;
        //}
    }
}
