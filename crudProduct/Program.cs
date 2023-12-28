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

app.MapGet("/product/{id}", async (DataContext context, int id) =>
await context.Products.FindAsync(id) is Product product ?
Results.Ok(product) : Results.NotFound("Product not found."));

app.MapPost("/product", async(DataContext context, Product product) =>
{
    if (string.IsNullOrWhiteSpace(product.Name))
        return Results.BadRequest("The Name field is required.");

    context.Products.Add(product);
    await context.SaveChangesAsync();
    return Results.Ok(product);
});

app.MapPut("/product/{id}", async(DataContext context, Product updateProduct, int id) =>
{
    var product = await context.Products.FindAsync(id);
    if (product is null)
        return Results.NotFound("Product not found.");

    if (string.IsNullOrWhiteSpace(updateProduct.Name))
        return Results.BadRequest("The Name field is required.");

    product.Name = updateProduct.Name;
    product.Description = updateProduct.Description;
    product.Price = updateProduct.Price;
    await context.SaveChangesAsync();

    return Results.Ok(product);
});

app.MapDelete("/product/{id}", async (DataContext context, int id) =>
{
    var product = await context.Products.FindAsync(id);
    if (product is null)
        return Results.NotFound("Product not found.");

    context.Products.Remove(product);
    await context.SaveChangesAsync();

    return Results.Ok(product);
});


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