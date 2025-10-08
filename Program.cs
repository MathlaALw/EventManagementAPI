using EventManagementAPI.Repo;
using EventManagementAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;
using EventManagementAPI.Middleware;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using Microsoft.OpenApi.Models;

namespace EventManagementAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ========= Serilog =========
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
            builder.Host.UseSerilog();

            // ========= DbContext =========
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString)
                       .UseLazyLoadingProxies());

            // ========= AutoMapper =========
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            // ========= DI: Repos & Services =========
            builder.Services.AddScoped(typeof(IGenericRepo<>), typeof(GenericRepo<>));
            builder.Services.AddScoped<IEventRepo, EventRepo>();
            builder.Services.AddScoped<IAttendeeRepo, AttendeeRepo>();

            builder.Services.AddScoped<IEventService, EventService>();
            builder.Services.AddScoped<IAttendeeService, AttendeeService>();
            builder.Services.AddScoped<IEventReportService, EventReportService>();

            // HttpClient (for weather integration)
            builder.Services.AddHttpClient();

            // ========= JWT Auth =========
            var jwtKey = builder.Configuration["Jwt:Key"]!;
            var jwtIssuer = builder.Configuration["Jwt:Issuer"];
            var jwtAudience = builder.Configuration["Jwt:Audience"];

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; // set true in prod with HTTPS
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    ValidateIssuer = true,
                    ValidIssuer = jwtIssuer,
                    ValidateAudience = true,
                    ValidAudience = jwtAudience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });

            builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

            // ========= Controllers =========
            builder.Services.AddControllers();

            // ========= Swagger =========
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "EventManagementAPI",
                    Version = "v1",
                    Description = "Events, Attendees, Reports with JWT authentication."
                });

                c.EnableAnnotations();

                // XML comments (guarded)
                var xmlName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlName);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
                }

                // Define Bearer ONCE
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter: Bearer {your JWT}"
                });

                // Reference the same id ONCE
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        Array.Empty<string>()
                    }
                });

             
            });

            // ========= Global Error Handling =========
            builder.Services.AddTransient<GlobalExceptionMiddleware>();

            // Uniform model validation response
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var problem = new ValidationProblemDetails(context.ModelState)
                    {
                        Type = "about:blank",
                        Title = "Validation failed",
                        Status = StatusCodes.Status400BadRequest,
                        Instance = context.HttpContext.Request.Path
                    };
                    problem.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
                    return new BadRequestObjectResult(problem)
                    {
                        ContentTypes = { "application/problem+json" }
                    };
                };
            });

            var app = builder.Build();

            // ========= Pipeline =========
            app.UseSerilogRequestLogging();

            // Keep Swagger enabled while you verify the doc renders
            app.UseSwagger();
            app.UseSwaggerUI(o =>
            {
                o.DocumentTitle = "Event Management API Docs";
                o.SwaggerEndpoint("https://localhost:7039/swagger/v1/swagger.json", "v1");
                o.DisplayRequestDuration();
                o.DefaultModelsExpandDepth(2);
            });

            app.UseHttpsRedirection();

            // Auth
            //app.UseAuthentication();   // must be before UseAuthorization
            app.UseAuthorization();

            // Global Exception Middleware
            app.UseMiddleware<GlobalExceptionMiddleware>();

            app.MapControllers();

            app.Run();
        }
    }
}
