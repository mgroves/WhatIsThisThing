using System.Text;
using Couchbase.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using WhatIsThisThing.Core;
using WhatIsThisThing.Core.Services;
using WhatIsThisThing.Server.Auth;

var builder = WebApplication.CreateBuilder(args);

// Replace the default configuration with the loaded configuration
var configuration = ConfigurationHelper.GetConfiguration();
builder.Configuration.AddConfiguration(configuration);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(x =>
    {
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtConfig:Issuer"],
            ValidAudience = builder.Configuration["JwtConfig:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JwtConfig:SecurityKey"]))
        };
    });

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCouchbase(builder.Configuration.GetSection("Couchbase"));
// builder.Services.AddCouchbase(options =>
// {
//     options.ConnectionString = "couchbase://localhost";
//     options.UserName = "Administrator";
//     options.Password = "password";
// });
builder.Services.AddTransient<IIdentifierService, IdentifierService>();
builder.Services.AddTransient<IEmbeddingService, AzureEmbeddingService>();
builder.Services.AddTransient<IDataLayer, DataLayer>();
builder.Services.AddTransient<AdminDataLayer>();
builder.Services.AddTransient<AdminServices>();
builder.Services.Configure<AzureComputerVisionSettings>(builder.Configuration.GetSection("AzureComputerVision"));
builder.Services.Configure<ImagebindSettings>(builder.Configuration.GetSection("Imagebind"));
builder.Services.AddTransient<TokenService>();
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
