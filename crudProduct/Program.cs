using crudProduct;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DataContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapGet("/product", async (DataContext context) =>
 await context.Products.OrderBy(p => p.Id).ToListAsync());

app.Run();

public class Product
{
    [Key]
    public int Id { get; set; }
    [Required(ErrorMessage = "The name field is required")]
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string? Description { get; set; }
}