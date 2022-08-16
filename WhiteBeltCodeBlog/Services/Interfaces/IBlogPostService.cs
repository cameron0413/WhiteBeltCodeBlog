using WhiteBeltCodeBlog.Models;

namespace WhiteBeltCodeBlog.Services.Interfaces
{
    public interface IBlogPostService
    {
        public Task<bool> ValidateSlugAsync(string title, int blogId);

        public Task<bool> IsTagOnBlogPostAsync(int tagId, int blogPostId);

        public Task AddTagToBlogPostAsync(int tagId, int blogPostId);

        public Task RemoveTagFromBlogPostAsync(int tagId, int blogPostId);

        public Task<IEnumerable<Tag>> GetBlogPostTagsAsync(int blogPostId);
        public Task<List<Category>> GetCategoriesAsync(int blogPostId);
        public Task<BlogPost> GetAllBlogPostsAsync(); // All posts regardless of IsDeleted or IsPublished
        public Task<BlogPost> GetPopularBlogPostsAsync(int count); //Defined by the number of comments made. The article with the most comments is the most popular.
        public Task<BlogPost> GetRecentBlogPostsAsync(int count); // Defined by the date created. The most recent articles in order of most to least recent will be displayed.


    }
}
