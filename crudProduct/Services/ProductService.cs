using crudProduct.Data;
using crudProduct.Interfaces;
using crudProduct.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace crudProduct.Services
{
    public class ProductService : IProductService
    {
        private readonly DataContext _context;

        public ProductService(DataContext context)
        {
            _context = context;
        }
        public async Task<IResult> CreateProductAsync(Product product)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(product.Name))
                    return Results.BadRequest("The Name field is required.");

                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return Results.Ok(product);
            }
            catch (Exception ex)
            {
                return Results.Problem("Internal error server: " + ex.Message);
            }
        }

        public async Task<IResult> DeleteProductByIdAsync(int id)
        {
            
        }

        public async Task<IResult> GetAllProductsAsync()
        {
            try
            {
                var listProducts = await _context.Products.OrderBy(p => p.Id).ToListAsync();
              return Results.Ok(listProducts);
            }
            catch (Exception ex)
            {
                return Results.Problem("Internal error server: " + ex.Message);
            }
        }

        public async Task<IResult> GetProductByIdAsync(int id)
        {
            try
            {
               return await _context.Products.FindAsync(id) is Product product ?
                    Results.Ok(product) : Results.NotFound("Product not found.");
            }
            catch(Exception ex)
            {
                return Results.Problem("Internal error server: " + ex.Message);
            }
        }

        public async Task<IResult> UpdateProductByIdAsync(Product updateProduct, int idProduct)
        {
            try
            {
                var product = await _context.Products.FindAsync(idProduct);
                if (product is null)
                    return Results.NotFound("Product not found.");

                if (string.IsNullOrWhiteSpace(updateProduct.Name))
                    return Results.BadRequest("The Name field is required.");

                product.Name = updateProduct.Name;
                product.Description = updateProduct.Description;
                product.Price = updateProduct.Price;
                await _context.SaveChangesAsync();

                return Results.Ok(product);
            }
            catch (Exception ex)
            {
                return Results.Problem("Internal error server: " + ex.Message);
            }

        }
    }
}
