
using CQRSMediator.Context;
using CQRSMediator.Entities;
using CQRSMediator.Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CQRSMediator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            //Configure applciaton log
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File("D:/MCH/PetProject/CQRSMediatR/ApplicationLog/Log.txt", 
                    rollingInterval: RollingInterval.Day)
                .CreateLogger();

            builder.Host.UseSerilog();

            // Add services to the container.
            builder.Services.AddMediatR(m => m.RegisterServicesFromAssembly(typeof(Program).Assembly));
            builder.Services.AddDbContext<ProductContext>(option => option.UseSqlServer(connectionString));
            builder.Services.AddDbContext<ApplicationDbContext>(option => option.UseSqlServer(connectionString));

            builder.Services.AddIdentity<Users, IdentityRole<int>>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddScoped<IPasswordHash, PasswordHash>();
            builder.Services.AddScoped<SignInManager<Users>>();
            builder.Services.AddScoped<UserManager<Users>>();
            builder.Services.AddControllers().AddJsonOptions(option =>
            {
                option.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                option.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                option.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHttpContextAccessor();
            builder.Services.Configure<IdentityOptions>(opt =>
            {
                // Password settings.
                //opt.Password.RequireDigit = true;
                //opt.Password.RequireLowercase = true;
                //opt.Password.RequireNonAlphanumeric = true;
                //opt.Password.RequireUppercase = true;
                //opt.Password.RequiredLength = 8;
                //opt.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
                opt.Lockout.MaxFailedAccessAttempts = 5;
                opt.Lockout.AllowedForNewUsers = true;

                // User settings.
                opt.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                opt.User.RequireUniqueEmail = true;
            });

            builder.Services.ConfigureApplicationCookie(opt =>
            {
                // Cookie settings
                opt.Cookie.HttpOnly = true;
                opt.ExpireTimeSpan = TimeSpan.FromMinutes(1);

                opt.LoginPath = "/api/UserAccount/SignIn";
                opt.AccessDeniedPath = "/";
                opt.SlidingExpiration = true;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

        #if DEBUG
            app.UseSwagger();
            app.UseSwaggerUI();
        #endif 
            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
