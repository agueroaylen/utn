using Dsw2025Tpi.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System.Text.Json;
using Dsw2025Tpi.Domain.Entities;

namespace Dsw2025Tpi.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // 🔐 JWT
        var jwtSettings = builder.Configuration.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };
        });

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            });
        });

        builder.Services.AddDbContext<Dsw2025TpiContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "API E-commerce", Version = "v1" });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Ingrese: Bearer {su token JWT}"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });
        });

        builder.Services.AddHealthChecks();

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<Dsw2025TpiContext>();

            // Usuario admin
            if (!context.Users.Any())
            {
                context.Users.Add(new User
                {
                    Id = Guid.NewGuid(),
                    Username = "admin",
                    Password = "admin123"
                });
            }

            // Clientes desde JSON
            var customerFilePath = Path.Combine(AppContext.BaseDirectory, "Data", "customers.json");
            Console.WriteLine($"📂 Ruta usada: {customerFilePath}");

            if (File.Exists(customerFilePath) && !context.Customers.Any())
            {
                var json = File.ReadAllText(customerFilePath);
                var customers = JsonSerializer.Deserialize<List<Customer>>(json);

                if (customers != null)
                {
                    Console.WriteLine($"👥 Clientes cargados: {customers.Count}");
                    context.Customers.AddRange(customers);
                }
                else
                {
                    Console.WriteLine("❌ Error al deserializar el archivo.");
                }

                context.SaveChanges(); // 💾 Importante: guardar cambios
            }
        }

        // Middlewares
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseMiddleware<Dsw2025Tpi.Api.Middleware.ErrorHandlingMiddleware>();
        app.UseHttpsRedirection();
        app.UseCors("AllowAll");
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.MapHealthChecks("/healthcheck");
        app.Run();
    }
}
