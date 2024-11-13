using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModsenPractice.Data; 
using ModsenPractice.Entity; 
using Microsoft.AspNetCore.Hosting;
using System.IO;
using ModsenPractice.Helpers;
using ModsenPractice.DTO;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using ModsenPractice.Patterns.UnitOfWork;

namespace ModsenPractice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUnitOfWork _unitOfWork;

        public UserController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegistrationDto registrationDto)
        {
            if (await _unitOfWork.UserRepository.AnyAsync(registrationDto.Email))
            {
                return BadRequest("User with this email already exists.");
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(registrationDto.Password);

            var newUser = new User
            {
                Username = registrationDto.Username,
                Email = registrationDto.Email,
                PasswordHash = passwordHash,
                RoleId = 1,
                RefreshToken = null,
                RefreshTokenExpiryTime = DateTime.Now
            };

            await _unitOfWork.UserRepository.AddAsync(newUser);
            await _unitOfWork.CommitAsync();

            return Ok("User registered successfully");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto loginDto)
        {
            var user = await _unitOfWork.UserRepository.GetByEmailAsync(loginDto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid email or password.");
            }

            var accessToken = GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(1);
            await _unitOfWork.CommitAsync();

            return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken });
        }

        [HttpGet("check-information")]
        [Authorize]
        public IActionResult CheckInformation()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return Unauthorized("You need to log in to view this page.");
            }

            var username = User.Identity?.Name;

            return Ok($"Hello {username}, this is a super cool page. You can see it because you have been logged in.");
        }


        [HttpGet("admin-resource")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult GetAdminResource()
        {
            return Ok("This is a protected admin resource.");
        }

        [HttpGet("throw-error")]
        public IActionResult ThrowError()
        {
            throw new InvalidOperationException("This is a test exception.");
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers(int page = 1, int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return BadRequest("Page and pageSize must be greater than zero.");
            }

            var users = await _unitOfWork.UserRepository.GetUsersAsync(page, pageSize);
            
            if (!users.Any())
            {
                return NotFound("No users found.");
            }

            return Ok(users);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(TokenRequestDto tokenRequest)
        {
            var user = await _unitOfWork.UserRepository.GetByRefreshTokenAsync(tokenRequest.RefreshToken);

            if (user == null || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return Unauthorized("Invalid or expired refresh token.");
            }
            
            var newAccessToken = GenerateAccessToken(user);

            return Ok(new { AccessToken = newAccessToken });
        }



        private string GenerateAccessToken(User user)
        {
            if (user.Role == null)
            {
                throw new InvalidOperationException("User role is not assigned.");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.roleName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Yhsfddbqyajsdismasdpfwuaxjdfreqsadadyur"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "ModsenPractice",
                audience: "User",
                claims: claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }
}