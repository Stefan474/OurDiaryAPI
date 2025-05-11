using OurDiaryAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Secret key for JWT (Replace with ENV variable in production)
var jwtSecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");

// Add services to the container
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=sharedDiary.db"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// JWT Authentication Configuration
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey))
    };
});

// Swagger Configuration
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "OurDiaryAPI", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\nExample: Bearer eyJhbGciOiJIUzI1NiIsInR..."
    });

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
            new string[] {}
        }
    });
});

var app = builder.Build();

// Middleware configuration

// Swagger Middleware (Only in Development)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HTTPS Redirection
app.UseHttpsRedirection();

// CORS (Optional - Configure if Angular is on a different port)
app.UseCors(builder =>
    builder.WithOrigins("http://localhost:4200")
           .AllowAnyHeader()
           .AllowAnyMethod()
);

// Authentication Middleware (Before Authorization)
app.UseAuthentication();

// Authorization Middleware
app.UseAuthorization();

// Map Controllers
app.MapControllers();

app.Run();
