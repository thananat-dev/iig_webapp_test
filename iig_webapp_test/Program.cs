using iig_webapp_test.Data;
using iig_webapp_test.Extensions;
using iig_webapp_test.Helpers;
using iig_webapp_test.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
// Configure DbContext 
builder.Services.AddDbContext<DatabaseContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("WebApiDatabase")));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
// configure DI for application services
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();
app.MigrateDatabase<Program>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(x => x
       .AllowAnyOrigin()
       .AllowAnyMethod()
       .AllowAnyHeader());

app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot")),
    RequestPath = new PathString("/wwwroot")
});


app.UseMiddleware<JwtMiddleware>();
app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();
