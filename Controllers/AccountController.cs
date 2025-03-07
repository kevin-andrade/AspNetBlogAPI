using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.Services;
using Blog.ViewModels;
using Blog.ViewModels.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasswordGenerator;
using System.Text.RegularExpressions;

namespace Blog.Controllers
{
    [ApiController]
    public class AccountController : ControllerBase
    {
        [HttpPost("v1/accounts/")]
        public async Task<IActionResult> PostAsync(
            [FromBody] RegisterViewModel model,
            [FromServices] EmailService emailService,
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

            bool emailSend = emailService.Send(
                user.Name,
                user.Email,
                "Welcome to the Blog!",
                $"Your password is <strong>{password}</strong>");

            if (!emailSend)
                return StatusCode(500, "01KE05 - Erro ao enviar o e-mail de boas-vindas.");

            try
            {
                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();

                return Ok(new ResultViewModel<dynamic>(new
                {
                    user = user.Email,
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

        [Authorize]
        [HttpPost("v1/accounts/upload-image")]
        public async Task<IActionResult> UploadImageAsync(
            [FromBody] UploadImageViewModel model,
            [FromServices] AppDataContext context)
        {
            var fileName = $"{Guid.NewGuid().ToString()}.jpg";
            var data = new Regex(@"data:image\/[a-z]+;base64,")
                .Replace(model.Base64Image, "");
            var bytesImage = Convert.FromBase64String(data);

            try
            {
                await System.IO.File.WriteAllBytesAsync($"wwwroot/images/{fileName}", bytesImage);
            }
            catch (Exception)
            {
                return StatusCode(500, new ResultViewModel<string>("Internal server error"));
            }

            var user = await context
                .Users
                .FirstOrDefaultAsync(x => x.Email == User.Identity.Name);

            if (user == null)
                return NotFound(new ResultViewModel<User>("User not found"));

            user.Image = $"https://localhost:0000/images/{fileName}"; 

            try
            {
                context.Users.Update(user);
                await context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return StatusCode(500, new ResultViewModel<string>("Internal server error"));
            }

            return Ok(new ResultViewModel<string>("Image altered successfully"));

        }
    }
}
