using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Data;
using Microsoft.Data.SqlClient;
using MyBlog.DataAccess.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using MyBlog.Business.Validators; // Validator s�n�flar�n� dahil ediyoruz

var builder = WebApplication.CreateBuilder(args);

// appsettings.json dosyas�ndaki JWT ayarlar�n� alal�m
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]); // Gizli anahtar

// Veritaban� ba�lant�s� i�in Connection String'i alal�m
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddTransient<IDbConnection>(db => new SqlConnection(connectionString)); // Veritaban� ba�lant�s�n� ekliyoruz

// JWT Authentication'� konfig�re edelim
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"], // Issuer do�rulamas�
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"], // Audience do�rulamas�
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero // Token'�n hemen s�resinin dolmas� i�in tolerans s�resi yok
    };
});

// Authorization Servisini ekleyelim
builder.Services.AddAuthorization();  // Authorization servisini ekliyoruz

// Swagger ve di�er servisleri ekleyin
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Repository ve Dependency Injection ekleyelim
builder.Services.AddScoped<IUserRepository, UserRepository>(); // IUserRepository ve UserRepository ba��ml�l�klar�n� ekliyoruz

// FluentValidation'� burada ekliyoruz
builder.Services.AddControllers()
    .AddFluentValidation(fv =>
        fv.RegisterValidatorsFromAssemblyContaining<LoginValidator>());

// Controller'lar� ekleyelim
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "My API",
        Version = "v1"
    });

    // Swagger anotasyonlar�n� etkinle�tir
    c.EnableAnnotations();
});

var app = builder.Build();

// Middleware'ler
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    // Swagger middleware'lerini ekleyelim
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My Blog API V1");
    });
}

app.UseHttpsRedirection();

app.UseRouting();

// Hata yakalama middleware'i burada ekleniyor
app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseAuthentication(); // JWT Authentication'� etkinle�tirir
app.UseAuthorization();  // Authorization'� etkinle�tirir

app.MapControllers(); // Controller'lar� haritalar ve �al��mas�n� sa�lar

app.Run();