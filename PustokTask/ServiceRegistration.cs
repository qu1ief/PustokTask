using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PPustokTask.Services;
using PustokTask.Data;
using PustokTask.Models;
using PustokTask.Services;
using PustokTask.Settings;

namespace PustokTask
{
	public static class ServiceRegistration
	{
		public static void AddService(this IServiceCollection services ,IConfiguration config)
		{
			services.AddControllersWithViews();
			
			services.AddHttpContextAccessor();
            services.ConfigureApplicationCookie(opt =>
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
			services.AddScoped<EmailService>();
			services.AddScoped<LayoutService>();
			services.Configure<EmailSetting>(config.GetSection("Email"));

			services.AddIdentity<AppUser, IdentityRole>(options =>
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
		}
	}
}
