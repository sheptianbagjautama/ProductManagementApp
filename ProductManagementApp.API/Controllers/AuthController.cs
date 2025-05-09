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
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserRepository userRepo, IUserService userService, IConfiguration config, ILogger<AuthController> logger)
        {
            _userRepo = userRepo;
            _userService = userService;
            _config = config;
            _logger = logger;
        }

        [HttpGet("test-error")]
        public IActionResult TestError()
        {
            try
            {
                throw new InvalidOperationException("This is a test error.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in TestError.");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid registration data: {@ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("Registering new user with username={Username}", dto.Username);

                var existingUser = await _userRepo.GetByUsernameAsync(dto.Username);
                if (existingUser != null)
                {
                    _logger.LogWarning("Username {Username} already exists.", dto.Username);
                    return BadRequest("Username already exists.");
                }

                _userService.CreatePasswordHash(dto.Password, out var hash, out var salt);

                var user = new User
                {
                    Username = dto.Username,
                    PasswordHash = hash,
                    PasswordSalt = salt
                };

                await _userRepo.AddAsync(user);
                var saved = await _userRepo.SaveChangesAsync();

                if (!saved)
                {
                    _logger.LogError("Failed to register user with username={Username}", dto.Username);
                    return BadRequest("Failed to register user.");
                }

                _logger.LogInformation("User with username={Username} registered successfully.", dto.Username);
                return StatusCode(201);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while registering user with username={Username}", dto.Username);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto dto)
        {
            try
            {
                _logger.LogInformation("Attempting login for username={Username}", dto.Username);

                var user = await _userRepo.GetByUsernameAsync(dto.Username);
                if (user == null)
                {
                    _logger.LogWarning("Login failed for username={Username}: Invalid credentials", dto.Username);
                    return Unauthorized("Invalid credentials");
                }

                var isValid = _userService.VerifyPasswordHash(dto.Password, user.PasswordHash, user.PasswordSalt);
                if (!isValid)
                {
                    _logger.LogWarning("Login failed for username={Username}: Invalid password", dto.Username);
                    return Unauthorized("Invalid credentials");
                }

                var token = JwtHelper.GenerateToken(user, _config);

                _logger.LogInformation("User with username={Username} logged in successfully.", dto.Username);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while attempting login for username={Username}", dto.Username);
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
