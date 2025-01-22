using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WebApplication1.Data;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
// Dodaj JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });

    // Konfiguracja JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Wpisz 'Bearer' i spację, a następnie token JWT.\n\nPrzykład: 'Bearer TwójTokenJWT'"
    });

    // Dodaj wymaganie zabezpieczeń
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// Konfiguracja CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()  // Zezwalaj na po��czenia z dowolnego �r�d�a
              .AllowAnyMethod()  // Zezwalaj na dowolne metody (GET, POST, PUT, DELETE)
              .AllowAnyHeader(); // Zezwalaj na dowolne nag��wki
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

// W��czenie CORS
app.UseCors("AllowAllOrigins");  // U�ywamy wcze�niej zdefiniowanej polityki CORS

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();  // Interfejs u�ytkownika Swagger
}
app.UseStaticFiles(); // Obsługa plików statycznych
app.UseRouting();
app.UseAuthentication(); // Dodaj uwierzytelnianie
app.UseAuthorization();  // Dodaj autoryzację
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
