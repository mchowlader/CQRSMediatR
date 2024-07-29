
using CQRSMediator.Context;
using CQRSMediator.Entities;
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

            builder.Services.AddControllers().AddJsonOptions(option =>
            {
                option.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                option.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                option.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

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

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
