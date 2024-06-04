using Couchbase.Extensions.DependencyInjection;
using WhatIsThisThing.Core.Services;
using WhatIsThisThing.Server.Services;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddCouchbase(options =>
{
    options.ConnectionString = "couchbase://localhost";
    options.UserName = "Administrator";
    options.Password = "password";
});
builder.Services.AddTransient<IIdentifierService, IdentifierService>();
builder.Services.AddTransient<IEmbeddingService, EmbeddingService>();
builder.Services.AddTransient<IDataLayer, DataLayer>();

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
