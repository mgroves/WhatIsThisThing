using Couchbase.Extensions.DependencyInjection;
using WhatIsThisThing.Core;
using WhatIsThisThing.Core.Services;

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
builder.Services.Configure<AzureComputerVisionSettings>(builder.Configuration.GetSection("AzureComputerVision"));
builder.Services.Configure<ImagebindSettings>(builder.Configuration.GetSection("Imagebind"));

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

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
