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
        // private static List<User> _users = new List<User>
        // {
        //     new User { Id = 1, Username = "user1", Email = "user1@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("hashedpassword1"), Role = new Role { roleID = 1, roleName = "User" }, RefreshToken = null, RefreshTokenExpiryTime = DateTime.Now },
        //     new User { Id = 2, Username = "user2", Email = "user2@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("hashedpassword2"), Role = new Role { roleID = 1, roleName = "User" }, RefreshToken = null, RefreshTokenExpiryTime = DateTime.Now },
        //     new User { Id = 3, Username = "user3", Email = "user3@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("hashedpassword3"), Role = new Role { roleID = 1, roleName = "User" }, RefreshToken = null, RefreshTokenExpiryTime = DateTime.Now },
        //     new User { Id = 4, Username = "admin", Email = "admin@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("hashedadminpassword"), Role = new Role { roleID = 2, roleName = "Admin" }, RefreshToken = null, RefreshTokenExpiryTime = DateTime.Now }
        // };
        
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegistrationDto registrationDto)
        {
            // Проверка на существование пользователя с таким email
            if (await _unitOfWork.UserRepository.AnyAsync(registrationDto.Email))
            {
                return BadRequest("User with this email already exists.");
            }

            // Хэширование пароля
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(registrationDto.Password);

            // Создание пользователя
            var newUser = new User
            {
                Username = registrationDto.Username,
                Email = registrationDto.Email,
                PasswordHash = passwordHash,
                RoleId = 1, // ID роли "User" (предполагается, что роли уже добавлены в БД)
                RefreshToken = null,
                RefreshTokenExpiryTime = DateTime.Now
            };

            // Сохранение пользователя в базе данных
            await _unitOfWork.UserRepository.AddAsync(newUser);
            await _unitOfWork.CommitAsync(); // Сохранение всех изменений

            return Ok("User registered successfully");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto loginDto)
        {
            // Проверка данных пользователя
            var user = await _unitOfWork.UserRepository.GetByEmailAsync(loginDto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid email or password.");
            }

            // Генерация Access и Refresh токенов
            var accessToken = GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken();

            // Сохранение refresh токена для пользователя
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(1); // Refresh токен действует 24 часа
            await _unitOfWork.CommitAsync(); // Сохранение всех изменений

            return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken });
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

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(TokenRequestDto tokenRequest)
        {
            // Найти пользователя по refresh токену
            var user = await _unitOfWork.UserRepository.GetByRefreshTokenAsync(tokenRequest.RefreshToken);

            // Проверка: существует ли пользователь с таким refresh токеном и не истек ли его срок действия
            if (user == null || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return Unauthorized("Invalid or expired refresh token.");
            }

            // Генерация нового access токена
            var newAccessToken = GenerateAccessToken(user);

            // Возвращение нового access токена
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