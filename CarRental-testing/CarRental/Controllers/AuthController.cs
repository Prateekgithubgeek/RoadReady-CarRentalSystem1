using Microsoft.AspNetCore.Mvc;
using CarRental.Models.DTOs; // Adjust namespace as needed


namespace CarRental.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtTokenService _jwtTokenService;

        public AuthController(JwtTokenService jwtTokenService)
        {
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginDTO request)
        {
            // Here, validate the user credentials by querying your database
            // For demonstration, let's assume validation is successful

            // Generate a JWT token
            var token = _jwtTokenService.GenerateToken(request.Email, request.Role);

            // Return the token wrapped in a response DTO
            var response = new UserLoginResponseDTO { Token = token };
            return Ok(response);
        }
    }
}
