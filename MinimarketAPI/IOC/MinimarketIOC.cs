using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Minimarket.Business.Service;
using Minimarket.Data;
using Minimarket.Data.Context;
using Minimarket.Data.Repo;
//using Microsoft.Extensions.DependencyInjection;

namespace Minimarket.IOC;

public static class MinimarketIOC
{
    public static void AddMinimarketServices(
        this IServiceCollection service, IConfiguration config
    )
    {
        // Connection String
        /*service.AddDbContext<MinimarketContext>(options =>
        {
            options.UseMySql(
                config.GetConnectionString("DBConnection"),
                ServerVersion.Parse("10.6.12-mariadb")
            );
        });*/

        service.AddDbContext<MinimarketContext>(options =>
        {
            options.UseSqlServer(
                config.GetConnectionString("DBConnection")
            );
        });


        // Repository.
        service.AddTransient(typeof(IGenericRepo<>), typeof(GenericRepo<>));

        // Sale.
        service.AddScoped<ISaleRepo, SaleRepo>();

        //Autommaper
        service.AddAutoMapper(typeof(AutoMapperProfile));

        // Services.
        service.AddScoped<IUserService, UserService>();
        service.AddScoped<IProductService, ProductService>();
        service.AddScoped<ISaleService, SaleService>();
        service.AddScoped<IDashBoardService, DashBoardService>();

        // Cors
        service.AddCors(options =>
        {
            options.AddPolicy(name: "CorsPolicy", app =>
            {
                var frontEndUri = config.GetConnectionString("FrontEndURI");
                app.SetIsOriginAllowed(origin => new Uri(origin).Host == frontEndUri)
                .AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
                //.AllowAnyHeader().AllowAnyMethod();
                //app.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            });
        });

        // JWT
        service.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,

                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Key"]!))
            };
        });
    }
}