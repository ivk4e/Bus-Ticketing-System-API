using BusifyAPI.Configurations;
using BusifyAPI.Data;
using BusifyAPI.Middlewares;
using BusifyAPI.Services.UserServices.Interfaces;
using BusifyAPI.Services.UserServices;
using Microsoft.Extensions.Options;
using System.Configuration;
using BusifyAPI.Repositories.Interfaces;
using BusifyAPI.Repositories;
using Microsoft.AspNetCore.Authentication;
using BusifyAPI.Services.AuthServices.Interfaces;
using BusifyAPI.Services.AuthServices;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IRegisterUserService, RegisterUserService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRegisterUserRepository, RegisterUserRepository>();
builder.Services.AddScoped<ILoginRepository, LoginRepository>();

builder.Services.AddSingleton<IAuthService, AuthService>();
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddScoped<ILoginUserService, LoginUserService>();

var jwtSecret = builder.Configuration["JwtSettings:Secret"];
builder.Services.AddSingleton(jwtSecret);

//Logger
LoggingConfig.ConfigureLogging(builder);

DbConfig.ConfigureDb(builder);

//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(options =>
//{
//    options.AddSecurityDefinition("ApiKeyAuth", new OpenApiSecurityScheme
//    {
//        Type = SecuritySchemeType.ApiKey,
//        In = ParameterLocation.Header,
//        Name = "X-ApiKey",
//        Description = "Please enter your API key in the field"
//    });

//    options.AddSecurityDefinition("ApiSecretAuth", new OpenApiSecurityScheme
//    {
//        Type = SecuritySchemeType.ApiKey,
//        In = ParameterLocation.Header,
//        Name = "X-ApiSecret",
//        Description = "Please enter your API secret in the field"
//    });

//    options.AddSecurityRequirement(new OpenApiSecurityRequirement
//    {
//        {
//            new OpenApiSecurityScheme
//            {
//                Reference = new OpenApiReference
//                {
//                    Type = ReferenceType.SecurityScheme,
//                    Id = "ApiKeyAuth"
//                }
//            },
//            new string[] { }
//        },
//        {
//            new OpenApiSecurityScheme
//            {
//                Reference = new OpenApiReference
//                {
//                    Type = ReferenceType.SecurityScheme,
//                    Id = "ApiSecretAuth"
//                }
//            },
//            new string[] { }
//        }
//    });
//});


var app = builder.Build();

var scope = app.Services.CreateScope();
var dbInitializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
dbInitializer.Initialize();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRequestLogging();
app.UseExceptionHandling();

app.UseHttpsRedirection();

app.UseAuthorization();
//app.UseApiKeyMiddleware();

app.MapControllers();

app.Run();
