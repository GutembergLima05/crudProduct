using crudProduct;
using crudProduct.Data;
using crudProduct.Interfaces;
using crudProduct.Models;
using crudProduct.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IProductService, ProductService>();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/user", async (IUserService userService, User user) =>
 await userService.CreateUserAsync(user));

app.MapPost("/login", async (IUserService userService, User user) =>
 await userService.LoginUserAsync(user, app.Services.GetRequiredService<TokenService>()));

app.MapGet("/product", async (IProductService productService) =>
 await productService.GetAllProductsAsync()).RequireAuthorization();

app.MapGet("/product/{id}", async (IProductService productService, int id) =>
await productService.GetProductByIdAsync(id)).RequireAuthorization();

app.MapPost("/product", async(IProductService productService, Product product) =>
await productService.CreateProductAsync(product)).RequireAuthorization();

app.MapPut("/product/{id}", async(IProductService productService, Product updateProduct, int idProduct) =>
await productService.UpdateProductByIdAsync(updateProduct, idProduct)).RequireAuthorization();

app.MapDelete("/product/{id}", async (IProductService productService, int id) =>
await productService.DeleteProductByIdAsync(id)).RequireAuthorization();

app.Run();