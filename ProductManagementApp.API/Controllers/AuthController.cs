using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductManagementApp.API.DTOs;
using ProductManagementApp.API.Helpers;
using ProductManagementApp.API.Models;
using ProductManagementApp.API.Repositories.Interfaces;
using ProductManagementApp.API.Services.Interfaces;

namespace ProductManagementApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly IUserService _userService;
        private readonly IConfiguration _config;

        public AuthController(IUserRepository userRepo, IUserService userService, IConfiguration config)
        {
            _userRepo = userRepo;
            _userService = userService;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = await _userRepo.GetByUsernameAsync(dto.Username);
            if (existingUser != null)
                return BadRequest("Username already exists.");

            _userService.CreatePasswordHash(dto.Password, out var hash, out var salt);

            var user = new User
            {
                Username = dto.Username,
                PasswordHash = hash,
                PasswordSalt = salt
            };

            await _userRepo.AddAsync(user);
            await _userRepo.SaveChangesAsync();

            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto dto)
        {
            var user = await _userRepo.GetByUsernameAsync(dto.Username);
            if (user == null)
                return Unauthorized("Invalid credentials");

            var isValid = _userService.VerifyPasswordHash(dto.Password, user.PasswordHash, user.PasswordSalt);
            if (!isValid)
                return Unauthorized("Invalid credentials");

            var token = JwtHelper.GenerateToken(user, _config);
            return Ok(new { token });
        }
    }
}
