using EventManagementAPI.Repo;
using EventManagementAPI.Services;
using Microsoft.EntityFrameworkCore;
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

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
