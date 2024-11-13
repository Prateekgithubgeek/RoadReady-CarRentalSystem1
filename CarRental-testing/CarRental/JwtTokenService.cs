using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CarRental
{
    public class JwtTokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(string email, string role)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");

            // Ensure that the key is at least 256 bits (32 bytes) long.
            var keyString = jwtSettings["Key"];
            var key = EnsureKeySize(keyString);

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Method to ensure the key is at least 256 bits
        private SymmetricSecurityKey EnsureKeySize(string key)
        {
            // Convert the key to bytes
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);

            // Ensure the key size is at least 256 bits (32 bytes)
            if (keyBytes.Length < 32)
            {
                throw new ArgumentException("The key must be at least 256 bits (32 bytes) in length.");
            }

            return new SymmetricSecurityKey(keyBytes);
        }
    }
}
