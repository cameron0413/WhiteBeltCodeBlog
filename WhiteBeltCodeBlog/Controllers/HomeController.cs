using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WhiteBeltCodeBlog.Data;
using WhiteBeltCodeBlog.Models;
using WhiteBeltCodeBlog.Services.Interfaces;

namespace WhiteBeltCodeBlog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IBlogPostService _blogPostService;

        public HomeController(ILogger<HomeController> logger,
                              ApplicationDbContext context,
                              IBlogPostService blogPostService)
        {
            _logger = logger;
            _context = context;
            _blogPostService = blogPostService;
        }

        public async Task<IActionResult> AuthorPage()
        {
            //To Do: Create Service to get blogposts

            List<BlogPost> posts = (await _blogPostService.GetAllBlogPostsAsync()).Where(p => p.IsPublished).ToList();
            return View(posts);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}