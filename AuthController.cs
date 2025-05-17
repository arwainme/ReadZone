using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReadZone.DTO;
using ReadZone.Models;
using ReadZone.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
namespace ReadZone.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (_context.Users.Any(u => u.Email == dto.Email))
                return BadRequest("Email already exists");

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                ProfileImageUrl = dto.ProfileImageUrl,
                BirthDate = dto.BirthDate
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok("User registered");
        }
        [AllowAnonymous]

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("Invalid credentials");

            var token = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                token,
                refreshToken,
                user.Id,
                user.Username,
                user.Email,
                user.ProfileImageUrl
            });
        }
        [Authorize(Roles = "Admin")]

        [HttpPost("addrole")]
        public async Task<IActionResult> AddRole(AddRoleDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null) return NotFound("User not found");

            user.Role = dto.Role;
            await _context.SaveChangesAsync();
            return Ok("Role updated");
        }

        [HttpPost("logout")]
        public IActionResult Logout() => Ok("Logged out");

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null) return NotFound();

            var otp = new Random().Next(100000, 999999).ToString();
            user.ResetOtp = otp;
            user.OtpExpiryTime = DateTime.UtcNow.AddMinutes(5);
            await _context.SaveChangesAsync();

            return Ok(new { otp });
        }
        [AllowAnonymous]
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp(VerifyOtpDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null || user.ResetOtp != dto.Otp || user.OtpExpiryTime < DateTime.UtcNow)
                return BadRequest("Invalid OTP");

            return Ok("OTP verified");
        }
        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null || user.ResetOtp != dto.Otp || user.OtpExpiryTime < DateTime.UtcNow)
                return BadRequest("Invalid OTP");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.ResetOtp = null;
            user.OtpExpiryTime = null;
            await _context.SaveChangesAsync();

            return Ok("Password reset");
        }
        [Authorize]
        [HttpGet("refreshToken")]
        public async Task<IActionResult> RefreshToken([FromQuery] string email, [FromQuery] string refreshToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime < DateTime.UtcNow)
                return Unauthorized();

            var newToken = GenerateJwtToken(user);
            var newRefresh = GenerateRefreshToken();

            user.RefreshToken = newRefresh;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();

            return Ok(new { token = newToken, refreshToken = newRefresh });
        }

        [HttpPost("revokeToken")]
        public async Task<IActionResult> RevokeToken([FromBody] string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return NotFound();

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await _context.SaveChangesAsync();

            return Ok("Revoked");
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role ?? "User")
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(_config["Jwt:DurationInMinutes"]!)),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }



}



