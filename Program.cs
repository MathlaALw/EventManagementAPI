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

            // Connect to SQL Server Database
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            //.UseLazyLoadingProxies());


            // Add AutoMapper
            builder.Services.AddAutoMapper(typeof(MappingProfile));
            // Adding TheRepo
            builder.Services.AddScoped(typeof(IGenericRepo<>), typeof(GenericRepo<>));
            builder.Services.AddScoped<IEventRepo, EventRepo>();
            builder.Services.AddScoped<IAttendeeRepo, AttendeeRepo>();

            // Adding the service 

            builder.Services.AddScoped<IEventService, EventService>();
            builder.Services.AddScoped<IAttendeeService, AttendeeService>();
            builder.Services.AddScoped<IEventReportService, EventReportService>();
            // Add services to the container.

            builder.Services.AddControllers();

            // Http Client Factory
            builder.Services.AddHttpClient();

         
            // JWT Auth
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
                options.RequireHttpsMetadata = false; // set true in production with HTTPS
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


            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            builder.Host.UseSerilog();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            // Swagger (add JWT support)
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "EventManagementAPI", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Enter: Bearer {your token}"
                });
                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
                // Enable [SwaggerOperation] 
                c.EnableAnnotations();

                // XML comments (controllers + models)
                var xmlName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlName);
                c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
                
                
                // JWT Bearer button in Swagger UI
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter: **Bearer {your JWT}**"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference
                { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
            });

            
            // register middleware
            builder.Services.AddTransient<GlobalExceptionMiddleware>();

            // Adding Unify model validation response -> to return the Error with details
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
            // Middleware integration for Serilog to automatically logs HTTP request/response pipeline
            app.UseSerilogRequestLogging();
            // Configure the HTTP request pipeline.
            // Swagger UI
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                // Swagger UI
                app.UseSwaggerUI(o =>
                {
                    o.DocumentTitle = "Event Management API Docs";
                    o.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                    o.DisplayRequestDuration();
                    o.DefaultModelsExpandDepth(2);
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            // Global Exception Middleware
            app.UseMiddleware<GlobalExceptionMiddleware>();

            app.MapControllers();

            app.Run();
        }
    }
}
