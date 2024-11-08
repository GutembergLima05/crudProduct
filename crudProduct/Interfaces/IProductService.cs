using crudProduct.Models;

namespace crudProduct.Interfaces
{
    public interface IProductService
    {
        Task<IResult> GetAllProductsAsync();
        Task<IResult> GetProductByIdAsync(int id);
        Task<IResult> CreateProductAsync(Product product);
        Task<IResult> UpdateProductByIdAsync(Product updateProduct, int idProduct);
        Task<IResult> DeleteProductByIdAsync(int id);
    }
}
