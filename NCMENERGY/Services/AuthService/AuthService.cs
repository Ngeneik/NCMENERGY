using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NCMENERGY.Data;
using NCMENERGY.Dtos;
using NCMENERGY.Models;
using NCMENERGY.Options;
using NCMENERGY.Response;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace NCMENERGY.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly Jwt _jwt;
        public AuthService(ApplicationDbContext context, IOptions<Jwt> jwt)
        {
            _context = context;
            _jwt = jwt.Value;
        }


        public async Task<GenericResponse> Login(Login request)
        {
            // 1. Find user by email
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
            {
                return new GenericResponse
                {
                    Success = false,
                    Status = "Error",
                    Message = "Invalid email or password"
                };
            }

            // 2. Verify password
            if (!VerifyPassword(request.Password, user.Password!))
            {
                return new GenericResponse
                {
                    Success = false,
                    Status = "Error",
                    Message = "Invalid email or password"
                };
            }

            // 3. Generate JWT token
            string token = GenerateJwtToken(user);

            // 4. Return success response
            return new GenericResponse
            {
                Success = true,
                Status = "Success",
                Message = "Login successful",
                Data = new
                {
                    user.Id,
                    user.Email,
                    Token = token
                }
            };
        }


        public async Task<GenericResponse> SignUp(SignUpDto request)
        {
            var settings = await _context.Settings.FirstOrDefaultAsync();
            if (settings == null || settings.AllowRegistrations == false)
            {
                return new GenericResponse
                {
                    Success = false,
                    Status = "Error",
                    Message = "Registrations are currently closed"
                };

            }


            if (request.Password != request.ConfirmPassword)
            {
                return new GenericResponse
                {
                    Success = false,
                    Status = "Error",
                    Message = "Passwords do not match"
                };
            }

            // Check if email already exists
            var existingUser = await _context.Users
                .Where(u => u.Email == request.Email).FirstOrDefaultAsync();
            if (existingUser != null)
            {
                return new GenericResponse
                {
                    Success = false,
                    Status = "Error",
                    Message = "Email already registered"
                };
            }

            // Hash the password
            string hashedPassword = HashPassword(request.Password);

            // Create user
            var user = new User
            {
                Email = request.Email,
                Password = hashedPassword,
                CreatedAt = DateTime.UtcNow
            };

            // Save to database
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new GenericResponse
            {
                Success = true,
                Status = "Success",
                Message = "User registered successfully",
                Data = new
                {
                    user.Id,
                    user.Email,
                    user.CreatedAt
                }
            };
        }

        private string HashPassword(string password)
        {
            // generate salt
            byte[] salt = RandomNumberGenerator.GetBytes(16);

            // derive key using static Pbkdf2
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                100000,
                HashAlgorithmName.SHA256,
                32
            );

            // combine salt + hash
            byte[] hashBytes = new byte[48];
            Buffer.BlockCopy(salt, 0, hashBytes, 0, 16);
            Buffer.BlockCopy(hash, 0, hashBytes, 16, 32);

            // convert to base64
            return Convert.ToBase64String(hashBytes);
        }

        private bool VerifyPassword(string password, string storedBase64)
        {
            byte[] hashBytes = Convert.FromBase64String(storedBase64);

            // extract salt
            byte[] salt = new byte[16];
            Buffer.BlockCopy(hashBytes, 0, salt, 0, 16);

            // hash incoming password
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                100000,
                HashAlgorithmName.SHA256,
                32
            );

            // compare
            for (int i = 0; i < 32; i++)
            {
                if (hashBytes[i + 16] != hash[i])
                    return false;
            }

            return true;
        }


        private string GenerateJwtToken(User user)
        {
            // JWT claims
            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            // Signing key
            var keyBytes = Encoding.UTF8.GetBytes(_jwt.Key);
            var securityKey = new SymmetricSecurityKey(keyBytes);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Token creation
            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwt.ExpiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
