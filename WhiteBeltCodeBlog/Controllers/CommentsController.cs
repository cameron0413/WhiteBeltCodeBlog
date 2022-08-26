using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WhiteBeltCodeBlog.Data;
using WhiteBeltCodeBlog.Extensions;
using WhiteBeltCodeBlog.Models;
using WhiteBeltCodeBlog.Services.Interfaces;

namespace WhiteBeltCodeBlog.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BlogUser> _userManager;

        public CommentsController(ApplicationDbContext context,
                                   UserManager<BlogUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

            // GET: Comments
            public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Comments.Include(c => c.Author).Include(c => c.BlogPost);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Comments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Comments == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.Author)
                .Include(c => c.BlogPost)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // GET: Comments/Create
        public async Task<IActionResult> Create()
        {
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["BlogPostId"] = new SelectList(_context.BlogPosts, "Id", "Content");
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BlogPostId,Body,Created")] Comment comment, string? slug)
        {
            ModelState.Remove("AuthorId");

            if (ModelState.IsValid)
            {
                comment.AuthorId = _userManager.GetUserId(User);
                comment.Created = DataUtility.GetPostgresDate(DateTime.Now);
                //DataUtility is a static class, which means that it does not need a using statement. It can be called from anywhere.
                _context.Add(comment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", "BlogPosts", new { slug });
        }

        // GET: Comments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Comments == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", comment.AuthorId);
            ViewData["BlogPostId"] = new SelectList(_context.BlogPosts, "Id", "Content", comment.BlogPostId);
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BlogPostId,AuthorId,Created,LastUpdated,UpdateReason,Body")] Comment comment)
        {
            if (id != comment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                try
                {
                    comment.Created = DateTime.SpecifyKind(comment.Created, DateTimeKind.Utc);
                    comment.LastUpdated = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
                    _context.Update(comment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("AuthorPage", "Home");
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", comment.AuthorId);
            ViewData["BlogPostId"] = new SelectList(_context.BlogPosts, "Id", "Content", comment.BlogPostId);
            return View(comment);
        }

        // GET: Comments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Comments == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.Author)
                .Include(c => c.BlogPost)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Comments == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Comments'  is null.");
            }
            var comment = await _context.Comments.FindAsync(id);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CommentExists(int id)
        {
          return (_context.Comments?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
