using DataAccessLayer.Data;
using Microsoft.Extensions.Configuration;
using DataAccessLayer.Filters;
using DataAccessLayer.Interface;
using DataAccessLayer.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Registre o ClinicaDbContext no contêiner de injeção de dependência.
builder.Services.AddDbContext<ClinicaDbContext>();
builder.Services.AddScoped<IUserInterface, UserRepository>();
builder.Services.AddScoped<IGenTokenFilter, GenTokenFilter>();
builder.Services.AddScoped<IDecToken, DecToken>();
builder.Services.AddScoped<IAppointInterface, AppointRepository>();
builder.Services.AddScoped<ILoginInterface, LoginRepository>();
// Add services Logger
var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.WriteIndented = true; // Opcional: formata o JSON de maneira legível
    });
#if USE_MY_TOKEN
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "JWTToken_Auth_API",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
#endif
// nao eh necessario a validação esta no servidor
//#if USE_MY_TOKEN
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
//{
//    options.RequireHttpsMetadata = false;
//    options.SaveToken = true;
//    options.TokenValidationParameters = new TokenValidationParameters()
//    {
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidAudience = builder.Configuration["Jwt:Audience"],
//        ValidIssuer = builder.Configuration["Jwt:Issuer"],
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
//    };
//});
//#endif

// add cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyCorsProfile",
    policy =>
    {
        policy
       .AllowAnyOrigin()
       .AllowAnyMethod()
       .AllowAnyHeader();
    });
});

var app = builder.Build();
app.UseCors("MyCorsProfile");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();




app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
