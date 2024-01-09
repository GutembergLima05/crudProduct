using crudProduct.Models;
using crudProduct.Services;

namespace crudProduct.Interfaces
{
    public interface IUserService
    {
        Task<IResult> CreateUserAsync(User user);
        Task<IResult> LoginUserAsync(User user, TokenService tokenService);

    }
}
