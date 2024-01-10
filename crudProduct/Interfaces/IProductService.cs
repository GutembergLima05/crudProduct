using crudProduct.Models;

namespace crudProduct.Interfaces
{
    public interface IProductService
    {
        Task<IResult> GetAllProductAsync();
    }
}
