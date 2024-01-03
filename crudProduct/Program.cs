using crudProduct;
using crudProduct.Data;
using crudProduct.Models;
using crudProduct.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.PrivateKey)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("manager"));
    options.AddPolicy("Employee", policy => policy.RequireRole("employee"));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DataContext>();
builder.Services.AddTransient<TokenService>();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/user", async (DataContext context, User user) =>
{
    if (user is null)
        return Results.BadRequest("Fill in all fields");

    if (string.IsNullOrWhiteSpace(user.Email))
        return Results.BadRequest("The Email field is required.");

    var userEmail = await context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
    if (userEmail != null)
        return Results.BadRequest("Email already exists");

    context.Add(user);
    await context.SaveChangesAsync();
    return Results.Ok(user);
});

app.MapPost("/login", async (User user, TokenService tokenService, DataContext context) =>
{
    if (user is null)
        return Results.BadRequest("Fill in all fields");

    var userLogged = await context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);

    if (userLogged is null)
        return Results.BadRequest("User not found");

    if (user.Password != userLogged.Password)
        return Results.BadRequest("User password wrong");

    return Results.Ok(tokenService.Generate(userLogged));
});

app.MapGet("/product", async (DataContext context) =>
 await context.Products.OrderBy(p => p.Id).ToListAsync()).RequireAuthorization();

app.MapGet("/product/{id}", async (DataContext context, int id) =>
await context.Products.FindAsync(id) is Product product ?
Results.Ok(product) : Results.NotFound("Product not found.")).RequireAuthorization();

app.MapPost("/product", async(DataContext context, Product product) =>
{
    if (string.IsNullOrWhiteSpace(product.Name))
        return Results.BadRequest("The Name field is required.");

    context.Products.Add(product);
    await context.SaveChangesAsync();
    return Results.Ok(product);
}).RequireAuthorization();

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
}).RequireAuthorization();

app.MapDelete("/product/{id}", async (DataContext context, int id) =>
{
    var product = await context.Products.FindAsync(id);
    if (product is null)
        return Results.NotFound("Product not found.");

    context.Products.Remove(product);
    await context.SaveChangesAsync();

    return Results.Ok(product);
}).RequireAuthorization();

app.Run();