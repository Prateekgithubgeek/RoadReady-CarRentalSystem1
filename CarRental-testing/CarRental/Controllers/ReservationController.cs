using AutoMapper;
using CarRental.DTOs; // Assuming the DTO is in this namespace
using CarRental.Exceptions;
using CarRental.Models;
using CarRental.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Require authentication for all actions
public class ReservationController : ControllerBase
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IMapper _mapper;

    public ReservationController(IReservationRepository reservationRepository, IMapper mapper)
    {
        _reservationRepository = reservationRepository;
        _mapper = mapper;
    }

    // GET: api/Reservations
    [HttpGet]
    [Authorize(Roles = "Admin")] // Allow Admin to get all reservations
    public async Task<IActionResult> GetAllReservations()
    {
        try
        {
            var reservations = await _reservationRepository.GetAllReservationsAsync();
            var reservationDTOs = _mapper.Map<IEnumerable<ReservationDTO>>(reservations); // Map to DTO
            return Ok(reservationDTOs);
        }
        catch (InternalServerException ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    // GET: api/Reservations/{reservationId}
    [HttpGet("{reservationId}")]
    [Authorize(Roles = "User, Admin")] // Allow both User and Admin to get specific reservation details
    public async Task<IActionResult> GetReservationById(int reservationId)
    {
        try
        {
            var reservation = await _reservationRepository.GetReservationByIdAsync(reservationId);
            if (reservation == null)
            {
                throw new NotFoundException("Reservation not found");
            }
            var reservationDTO = _mapper.Map<ReservationDTO>(reservation); // Map to DTO
            return Ok(reservationDTO);
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

    // POST: api/Reservations
    [HttpPost]
    [Authorize(Roles = "User")] // Only Users can create their reservations
    public async Task<IActionResult> AddReservation([FromBody] ReservationDTO reservationDTO)
    {
        try
        {
            var reservation = _mapper.Map<Reservation>(reservationDTO); // Map from DTO to model
            await _reservationRepository.AddReservationAsync(reservation);
            var createdReservationDTO = _mapper.Map<ReservationDTO>(reservation); // Map to DTO for response
            return CreatedAtAction(nameof(GetReservationById), new { reservationId = reservation.ReservationId }, createdReservationDTO);
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

    // PUT: api/Reservations
    [HttpPut]
    [Authorize(Roles = "User, Admin")] // Allow Users to update their own reservations and Admin to update any reservation
    public async Task<IActionResult> UpdateReservation([FromBody] ReservationDTO reservationDTO)
    {
        try
        {
            var reservation = _mapper.Map<Reservation>(reservationDTO); // Map from DTO to model
            await _reservationRepository.UpdateReservationAsync(reservation);
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

    // DELETE: api/Reservations/{reservationId}
    [HttpDelete("{reservationId}")]
    [Authorize(Roles = "User, Admin")] // Allow Users to delete their own reservations and Admin to delete any reservation
    public async Task<IActionResult> DeleteReservation(int reservationId)
    {
        try
        {
            await _reservationRepository.DeleteReservationAsync(reservationId);
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

    // GET: api/Reservations/user/{userId}
    [HttpGet("user/{userId}")]
    [Authorize(Roles = "User, Admin")] // Allow both User and Admin to view reservations by user
    public async Task<IActionResult> GetReservationsByUserId(int userId)
    {
        try
        {
            var reservations = await _reservationRepository.GetReservationsByUserIdAsync(userId);
            var reservationDTOs = _mapper.Map<IEnumerable<ReservationDTO>>(reservations); // Map to DTO
            return Ok(reservationDTOs);
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

    // GET: api/Reservations/car/{carId}
    [HttpGet("car/{carId}")]
    [Authorize(Roles = "User, Admin")] // Allow both User and Admin to view reservations by car
    public async Task<IActionResult> GetReservationsByCarId(int carId)
    {
        try
        {
            var reservations = await _reservationRepository.GetReservationsByCarIdAsync(carId);
            var reservationDTOs = _mapper.Map<IEnumerable<ReservationDTO>>(reservations); // Map to DTO
            return Ok(reservationDTOs);
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
}
