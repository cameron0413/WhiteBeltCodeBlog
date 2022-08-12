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

    }
}
