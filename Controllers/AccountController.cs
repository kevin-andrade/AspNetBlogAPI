using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.Services;
using Blog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasswordGenerator;

namespace Blog.Controllers
{
    [ApiController]
    public class AccountController : ControllerBase
    {
        [HttpPost("v1/accounts/")]
        public async Task<IActionResult> PostAsync(
            [FromBody] RegisterViewModel model,
            [FromServices] AppDataContext context)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

            var user = new User
            {
                Name = model.Name,
                Email = model.Email,
                Slug = model.Email.Replace("@", "-").Replace(".", "-"),
                Bio = model.Bio,
                Image = model.Image,
            };

            var password = new Password(
                passwordLength: 25,
                includeLowercase: true,
                includeNumeric: true,
                includeSpecial: true,
                includeUppercase: true)
                .Next();
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password); // Entering User Password

            try
            {
                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();

                return Ok(new ResultViewModel<dynamic>(new
                {
                    user = user.Email,
                    password
                }));
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "01KE03 - Error in the database");
            }
        }

        [HttpPost("v1/accounts/login")]
        public async Task<IActionResult> LoginAsync(
            [FromBody] LoginViewModel model,
            [FromServices] AppDataContext context,
            [FromServices] TokenService tokenService)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

            var user = await context
                .Users
                .AsNoTracking()
                .Include(x => x.Roles)
                .FirstOrDefaultAsync(x => x.Email == model.Email);

            if (user == null)
                return StatusCode(401, new ResultViewModel<string>("Invalid username or password"));

            var passwordHashValid = BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash);
            if (!passwordHashValid)
                return StatusCode(401, new ResultViewModel<string>("Invalid username or password"));

            try
            {
                var token = tokenService.GenerateToken(user);
                return Ok(new ResultViewModel<string>(token, null));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<string>("Internal server error"));
            }
        }
    }
}
