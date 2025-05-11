using Microsoft.AspNetCore.Mvc;
using OurDiaryAPI.Data;
using OurDiaryAPI.DTOs;
using OurDiaryAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;


namespace OurDiaryAPI.Controllers
{

    [ApiController]
    [Route("api/auth")] // Explicitly set to "auth"
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        private readonly byte[] _key;

        public AuthController(AppDbContext context)
        {
            _context = context;

            string secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
            if (string.IsNullOrEmpty(secretKey))
                throw new InvalidOperationException("JWT_SECRET_KEY environment variable not set.");

            _key = Encoding.UTF8.GetBytes(secretKey);
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return BadRequest("Email already registered.");

            using var hmac = new HMACSHA256(_key);
            var user = new User
            {
                Email = dto.Email,
                PasswordHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password)))
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User registered.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null) return Unauthorized("Invalid credentials.");

            using var hmac = new HMACSHA256(_key);
            var computedHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password)));

            if (user.PasswordHash != computedHash)
                return Unauthorized("Invalid credentials.");

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { Token = tokenString });
        }
    
        [Authorize]
        [HttpGet("user")]
        public async Task<IActionResult> GetUser()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (userEmail == null) return Unauthorized("User not found.");
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null) return Unauthorized("User not found.");
            return Ok(new
            {
                Email = user.Email,
                IsConnected = user.isConnected,
                PartnerEmail = user.partnerEmail
            });
        }

        [Authorize]
        [HttpPost("connect-partner")]
        public async Task<IActionResult> ConnectPartner([FromBody] PartnerDto dto)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (userEmail == null) return Unauthorized("User not found.");

            if (dto.PartnerEmail == userEmail)
                return BadRequest("You cannot connect with yourself.");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null) return Unauthorized("User not found.");

            var partner = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.PartnerEmail);

            if (partner == null)
                return NotFound("Partner not found.");

            // Check if the partner is already connected to someone else
            if (partner.isConnected && partner.partnerEmail != userEmail)
                return BadRequest("Partner is already connected to someone else.");

            // If the partner has already selected the current user, finalize the connection
            if (partner.partnerEmail == userEmail)
            {
                user.isConnected = true;
                partner.isConnected = true;
                user.partnerEmail = dto.PartnerEmail;
            }
            else
            {
                // Otherwise, just set the partner email and wait for acceptance
                user.partnerEmail = dto.PartnerEmail;
            }

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    Message = user.isConnected
                        ? $"You are now connected with {dto.PartnerEmail}!"
                        : $"Partner request sent to {dto.PartnerEmail}. Waiting for their confirmation."
                });
            }
            catch
            {
                return StatusCode(500, "Error updating partner connection.");
            }
        }


    }
}