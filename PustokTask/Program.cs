using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PustokTask.Data;
using PustokTask.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<PustokDbContex>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("MvcProject"));
});

builder.Services.ConfigureApplicationCookie(opt =>
{
    opt.Events.OnRedirectToLogin = opt.Events.OnRedirectToAccessDenied = context =>
    {
        var uri = new Uri(context.RedirectUri);

        if (context.Request.Path.Value.ToLower().StartsWith("/manage"))
        {
            context.Response.Redirect("/manage/account/login" + uri.Query);
        }
        else
        {
            context.Response.Redirect("/account/login" + uri.Query);
        }

        return Task.CompletedTask;
    };
});


builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;

    options.Lockout.MaxFailedAccessAttempts = 3;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
    options.Lockout.AllowedForNewUsers = true;

}).AddEntityFrameworkStores<PustokDbContex>().AddDefaultTokenProviders();
// Configure the HTTP request pipeline.

var app = builder.Build();

if (!app.Environment.IsDevelopment())
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
            name: "areas",
            pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
          );
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
