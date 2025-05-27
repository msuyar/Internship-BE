using AspNETWebAPIDersleri.Repository;
using LMS.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Cors Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", buil =>
    {
        //Allows only Get Methods bu with in yeri değişerek seçicilik değiştirilebilir
        //buil.AllowAnyOrigin().WithMethods("Get").AllowAnyHeader();
        //buil.WithOrigins("http://localhost:3000").WithMethods("Get").AllowAnyHeader();
        buil.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();

// DBConfig
builder.Services.AddDbContext<LMSDBContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString(
        "Host=localhost;Port=5433;Username=postgres;Password=mysecretpassword;Database=task1db"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("AllowAll");

app.MapControllers();

app.Run();