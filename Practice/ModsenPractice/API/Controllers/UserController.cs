using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModsenPractice.DTO;
using ModsenPractice.Services;

namespace ModsenPractice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegistrationDto registrationDto)
        {
            var isRegistered = await _userService.RegisterUser(registrationDto);
            if (!isRegistered)
            {
                return BadRequest("User with this email already exists.");
            }

            return Ok("User registered successfully");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto loginDto)
        {
            var (isValid, accessToken, refreshToken) = await _userService.LoginUser(loginDto);

            if (!isValid)
            {
                return Unauthorized("Invalid email or password.");
            }

            return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken });
        }

        [HttpGet("check-information")]
        [Authorize]
        public IActionResult CheckInformation()
        {
            var username = User.Identity?.Name;
            return Ok($"Hello {username}, this is a super cool page. You can see it because you have been logged in.");
        }

        [HttpGet("admin-resource")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult GetAdminResource()
        {
            return Ok("This is a protected admin resource.");
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers(int page = 1, int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return BadRequest("Page and pageSize must be greater than zero.");
            }

            var users = await _userService.GetAllUsers(page, pageSize);

            if (!users.Any())
            {
                return NotFound("No users found.");
            }

            return Ok(users);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(TokenRequestDto tokenRequest)
        {
            var newAccessToken = await _userService.RefreshToken(tokenRequest.RefreshToken);

            if (newAccessToken == null)
            {
                return Unauthorized("Invalid or expired refresh token.");
            }

            return Ok(new { AccessToken = newAccessToken });
        }
    }
}
