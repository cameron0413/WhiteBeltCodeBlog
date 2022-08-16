﻿using WhiteBeltCodeBlog.Services.Interfaces;
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
                                                 .Include(b => b.Comments)
                                                    .ThenInclude(b =>b.Author)
                                                 .Include(b => b.Category)
                                                 .Include(b => b.Tags)
                                                 .ToListAsync();

            return blogPosts;
        }

        public async Task<BlogPost> GetPopularBlogPostsAsync(int count)
        {
            List<BlogPost> blogPosts = await _context.BlogPosts.Include(b => b.Comments).ToListAsync();
            int mostComments = 0;

            for (int i = 0; i < blogPosts.Count; i++)
            {
                int commentsNumber = blogPosts[i].Comments.Count;

                if (commentsNumber > mostComments)
                {
                    mostComments = commentsNumber;
                }
            }
        }

        public async Task<BlogPost> GetRecentBlogPostsAsync(int count)
        {

        }
    }
}
