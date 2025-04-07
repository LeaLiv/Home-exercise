using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using PartD.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// builder.Services.AddOpenApi();

builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
{
    builder.WithOrigins("*")
           .AllowAnyMethod()
           .AllowAnyHeader();

}));
builder.Services.AddSupplier();
builder.Services.AddOrderService();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.MapOpenApi();
}
BsonSerializer.RegisterIdGenerator(typeof(string), new StringObjectIdGenerator());
// app.UseDefaultFiles();
app.UseCors("MyPolicy");
// app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
