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

        // Add services to the container.
        //  var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        var mappingConfiguration = new MapperConfiguration(m => m.AddProfile(new MProfile()));
        IMapper mapper = mappingConfiguration.CreateMapper();
        builder.Services.AddSingleton(mapper);
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddServices(builder.Configuration);

        builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
            .AddEntityFrameworkStores<ApplicationDbContext>();
        builder.Services.AddControllersWithViews();

        var app = builder.Build();

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
        app.UseRouting();

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
