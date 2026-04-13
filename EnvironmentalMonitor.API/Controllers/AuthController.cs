using Microsoft.AspNetCore.Mvc;
using EnvironmentalMonitor.API.Services;
using EnvironmentalMonitor.API.Models;
using BCrypt.Net;

namespace EnvironmentalMonitor.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class AuthController: ControllerBase
    {
        private readonly UserService _userService = new UserService();

        // Register
        [HttpPost("register")]

        public IActionResult Register([FromBody] User newUser)
        {
            var users = _userService.GetUsers();

            if (users.Any(u => u.Username == newUser.Username))
            {
                return BadRequest("Username already exists.");
            }

            newUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newUser.PasswordHash);
            newUser.Theme = "light";

            users.Add(newUser);
            _userService.SaveUsers(users);

            return Ok("User created successfully.");
        }

        // Login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest login)
        {
            var users = _userService.GetUsers();

            var user = users.FirstOrDefault(u => u.Username == login.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(login.PasswordHash, user.PasswordHash))
            {
                return Unauthorized("Invalid username or password.");
            }

            return Ok(user);
        }
    }
}
