using AutoMapper;
using CarRental.DTOs;
using CarRental.Exceptions;
using CarRental.Models;
using CarRental.Models.DTOs;
using CarRental.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;  // For PasswordHasher
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Enforce authentication for all actions in this controller
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserController(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    // GET: api/Users
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        try
        {
            var users = await _userRepository.GetAllUsersAsync();
            var userDTOs = _mapper.Map<IEnumerable<UserDTO>>(users); // Map to DTOs
            return Ok(userDTOs);
        }
        catch (InternalServerException ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    // GET: api/Users/{userId}
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserById(int userId)
    {
        try
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }
            var userDTO = _mapper.Map<UserDTO>(user); // Map to DTO
            return Ok(userDTO);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InternalServerException ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    // POST: api/Users (User Registration)
    [HttpPost]
    [Authorize(Roles = "Admin")] // Only Admins can add users
    public async Task<IActionResult> AddUser([FromBody] UserRegistrationDTO userRegistrationDTO)
    {
        try
        {
            var user = _mapper.Map<User>(userRegistrationDTO); // Map DTO to User model
            await _userRepository.AddUserAsync(user);
            var createdUserDTO = _mapper.Map<UserDTO>(user); // Map to DTO for response
            return CreatedAtAction(nameof(GetUserById), new { userId = user.UserId }, createdUserDTO);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (DuplicateResourceException ex)
        {
            return Conflict(ex.Message);
        }
        catch (InternalServerException ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    // PUT: api/Users (User Update)
    [HttpPut]
    [Authorize(Roles = "Admin, User")] // Admins and the user themselves can update user details
    public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDTO userUpdateDTO)
    {
        try
        {
            var user = _mapper.Map<User>(userUpdateDTO); // Map DTO to User model
            await _userRepository.UpdateUserAsync(user);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InternalServerException ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    // DELETE: api/Users/{userId}
    [HttpDelete("{userId}")]
    [Authorize(Roles = "Admin")] // Only Admins can delete users
    public async Task<IActionResult> DeleteUser(int userId)
    {
        try
        {
            await _userRepository.DeleteUserAsync(userId);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InternalServerException ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("login")]
    [AllowAnonymous] // Allow anyone to log in (doesn't require authentication)
    public async Task<IActionResult> Login([FromBody] UserLoginDTO userLoginDTO)
    {
        try
        {
            var user = await _userRepository.GetUserByEmailAsync(userLoginDTO.Email);
            if (user == null)
            {
                return Unauthorized("Invalid email or password");
            }

            // Skip the password verification logic (removed PasswordHasher and verification step)

            // Generate JWT token
            var token = GenerateJwtToken(user);
            return Ok(new { Token = token });
        }
        catch (InternalServerException ex)
        {
            return StatusCode(500, ex.Message);
        }
    }


    private string GenerateJwtToken(User user)
    {
        // Implement JWT generation logic here
        return "sample-jwt-token"; // This is just a placeholder
    }
}
