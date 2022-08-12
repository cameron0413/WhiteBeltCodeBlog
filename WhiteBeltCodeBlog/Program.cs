using Microsoft.AspNetCore.Identity;
using WhiteBeltCodeBlog.Models;
using Microsoft.EntityFrameworkCore;
using WhiteBeltCodeBlog.Data;
using WhiteBeltCodeBlog.Services.Interfaces;
using WhiteBeltCodeBlog.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = DataUtility.GetConnectionString(builder.Configuration);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

//later on, we will add "split queries" here. Ask Antonio what that means.

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<BlogUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddDefaultUI()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();

// Custom Service
builder.Services.AddScoped<IImageService,ImageService>();
builder.Services.AddScoped<IBlogPostService, BlogPostService>();


builder.Services.AddMvc();
//Why don't we just always use AddMvc()?

var app = builder.Build();
var scope = app.Services.CreateScope();
await DataUtility.SeedDataAsync(scope.ServiceProvider);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "custom",
    pattern: "Content/{slug}",
    defaults: new
    {
        controller = "BlogPosts",
        action = "Details"
    });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
