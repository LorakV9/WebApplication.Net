using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;

var builder = WebApplication.CreateBuilder(args);

// Konfiguracja CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()  // Zezwalaj na po³¹czenia z dowolnego Ÿród³a
              .AllowAnyMethod()  // Zezwalaj na dowolne metody (GET, POST, PUT, DELETE)
              .AllowAnyHeader(); // Zezwalaj na dowolne nag³ówki
    });
});

// Konfiguracja EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    new MySqlServerVersion(new Version(8, 0, 33))));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var app = builder.Build();

// W³¹czenie CORS
app.UseCors("AllowAllOrigins");  // U¿ywamy wczeœniej zdefiniowanej polityki CORS

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();  // Interfejs u¿ytkownika Swagger
}

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
