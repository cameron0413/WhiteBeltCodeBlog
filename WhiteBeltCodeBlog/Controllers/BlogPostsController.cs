using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WhiteBeltCodeBlog.Data;
using WhiteBeltCodeBlog.Extensions;
using WhiteBeltCodeBlog.Models;
using WhiteBeltCodeBlog.Services;
using WhiteBeltCodeBlog.Services.Interfaces;

namespace WhiteBeltCodeBlog.Controllers
{
    public class BlogPostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BlogUser> _userManager;
        private readonly IImageService _imageService;
        private readonly IBlogPostService _blogPostService;

        public BlogPostsController(ApplicationDbContext context,
                                   IImageService imageService,
                                   UserManager<BlogUser> userManager,
                                   IBlogPostService blogPostService)
        {
            _context = context;
            _userManager = userManager;
            _imageService = imageService;
            _blogPostService = blogPostService;
            //Ask again why we use these private variables, when they are made equivalent to 
        }

        // GET: BlogPosts
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.BlogPosts.Include(b => b.Category).Include(b => b.Tags);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: BlogPosts/Details/5
        public async Task<IActionResult> Details(string? slug)
        {

            if (string.IsNullOrEmpty(slug))
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts
                .Include(b => b.Category)
                .Include(b =>b.Comments)
                    .ThenInclude(c =>c.Author)
                .Include(b => b.Tags)
                .FirstOrDefaultAsync(m => m.Slug == slug);
            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        // GET: BlogPosts/Create
        public IActionResult Create()
        {
            string blogUserId = _userManager.GetUserId(User);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["TagList"] = new MultiSelectList(_context.Tags, "Id", "Name");
            return View();
        }

        // POST: BlogPosts/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CategoryId,Title,Content,LastUpdated,Slug,Abstract,BlogPostImage,TagList,")] BlogPost blogPost, List<int> TagList, List<int> selectedTags)
        {
            if (ModelState.IsValid)
            {
                //Date(s)
                blogPost.Created = DataUtility.GetPostgresDate(DateTime.Now);
                //Slug ??
                if (!await _blogPostService.ValidateSlugAsync(blogPost.Title,blogPost.Id))
                {
                    ModelState.AddModelError("Title", "A similar title or slug has already been used!");

                    ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", blogPost.CategoryId);
                    ViewData["TagList"] = new MultiSelectList(_context.Tags, "Id", "Name", blogPost.Tags);
                    return View(blogPost);
                }

                blogPost.Slug = blogPost.Title!.Sluggify();
                
                //Image
                if (blogPost.BlogPostImage != null)
                {
                    blogPost.ImageData = await _imageService.ConvertFileToByteArrayAsync(blogPost.BlogPostImage);
                    blogPost.ImageType = blogPost.BlogPostImage.ContentType;
                }

                // blogPost.Created = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
                // I commented out the last line because this is what we had for the address book app. The DataUtility is doing this for us now though.

                blogPost.Created = DataUtility.GetPostgresDate(DateTime.Now);


                
                
                foreach (int tagId in selectedTags)
                {
                    blogPost.Tags.Add(_context.Tags.Find(tagId)!);
                }
               



                _context.Add(blogPost);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }



            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", blogPost.CategoryId);
            ViewData["TagList"] = new MultiSelectList(_context.Tags, "Id", "Name", blogPost.Tags);
            return View(blogPost);
        }

        // GET: BlogPosts/Edit/5
        public async Task<IActionResult> Edit(int? id, List<int> selectedTags)
        {
            if (id == null || _context.BlogPosts == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts.Include(b => b.Tags).FirstOrDefaultAsync(b => b.Id == id);
            if (blogPost == null)
            {
                return NotFound();
            }

            

            blogPost.Tags.Clear();

            foreach (int tagId in selectedTags)
            {
                blogPost.Tags.Add(_context.Tags.Find(tagId)!);
            }


            string blogUserId = _userManager.GetUserId(User);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["TagList"] = new MultiSelectList(_context.Tags, "Id", "Name", selectedTags);
            return View(blogPost);
        }

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CategoryId,Title,Content,LastUpdated,Slug,Abstract,BlogPostImage,TagList,")] BlogPost blogPost, List<int> selectedTags)
        {
            if (id != blogPost.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    blogPost.Created = DateTime.SpecifyKind(blogPost.Created, DateTimeKind.Utc);
                    blogPost.LastUpdated = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
                    

                    if (blogPost.BlogPostImage != null)
                    {
                        blogPost.ImageData = await _imageService.ConvertFileToByteArrayAsync(blogPost.BlogPostImage);
                        blogPost.ImageType = blogPost.BlogPostImage.ContentType;
                    }

                    _context.Update(blogPost);

                    //Look at the edit method in the address book to see what code goes here.

                    //Remove old tags
                    List<Tag> oldTags = (await _blogPostService.GetBlogPostTagsAsync(blogPost.Id)).ToList();


                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogPostExists(blogPost.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));

               

            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["TagList"] = new MultiSelectList(_context.Tags, "Id", "Name", selectedTags);
            return View(blogPost);
        }

        // GET: BlogPosts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.BlogPosts == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        // POST: BlogPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.BlogPosts == null)
            {
                return Problem("Entity set 'ApplicationDbContext.BlogPosts'  is null.");
            }
            var blogPost = await _context.BlogPosts.FindAsync(id);
            if (blogPost != null)
            {
                _context.BlogPosts.Remove(blogPost);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogPostExists(int id)
        {
          return (_context.BlogPosts?.Any(e => e.Id == id)).GetValueOrDefault();
        }

    }
}
