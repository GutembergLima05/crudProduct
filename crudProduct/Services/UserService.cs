using crudProduct.Data;
using crudProduct.Interfaces;
using crudProduct.Models;
using Microsoft.EntityFrameworkCore;

namespace crudProduct.Services
{
    public class UserService: IUserService
    {
        private readonly DataContext _context;

        public UserService(DataContext context)
        {
            _context = context;
        }

        public async Task<IResult> CreateUserAsync(User user)
        {
            if (user is null)
                return Results.BadRequest("Fill in all fields");

            if (string.IsNullOrWhiteSpace(user.Email))
                return Results.BadRequest("The Email field is required.");

            var userEmail = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (userEmail != null)
                return Results.BadRequest("Email already exists");

            _context.Add(user);
            await _context.SaveChangesAsync();
            return Results.Ok(user);
        }


        public async Task<IResult> LoginUserAsync(User user, TokenService tokenService)
        {
            if (user is null)
                return Results.BadRequest("Fill in all fields");

            var userLogged = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);

            if (userLogged is null)
                return Results.BadRequest("User not found");

            if (user.Password != userLogged.Password)
                return Results.BadRequest("User password wrong");

            return Results.Ok(tokenService.Generate(userLogged));
        }
    }
}
