using Application;
using AutoMapper;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Services.Common;
using Microsoft.EntityFrameworkCore;

namespace MvcTemplate;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        var mappingConfiguration = new MapperConfiguration(m => m.AddProfile(new MProfile()));
        IMapper mapper = mappingConfiguration.CreateMapper();
        builder.Services.AddSingleton(mapper);
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddServices(builder.Configuration);

        builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
            .AddEntityFrameworkStores<ApplicationDbContext>();

        // ❌ Eliminamos política de autorización global
        builder.Services.AddControllersWithViews();

        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Identity/Account/Login";
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapStaticAssets();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        app.MapRazorPages()
           .WithStaticAssets();

        app.Run();
    }
}
