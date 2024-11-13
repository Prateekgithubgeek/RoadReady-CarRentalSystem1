using AutoMapper;
using CarRental.DTOs; // Assuming the ReviewDTO is in this namespace
using CarRental.Exceptions;
using CarRental.Models;
using CarRental.Models.DTOs;
using CarRental.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Require authentication for all actions
public class ReviewController : ControllerBase
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IMapper _mapper;

    public ReviewController(IReviewRepository reviewRepository, IMapper mapper)
    {
        _reviewRepository = reviewRepository;
        _mapper = mapper;
    }

    // GET: api/Reviews
    [HttpGet]
    [Authorize(Roles = "User, Admin")] // Allow both User and Admin to get all reviews
    public async Task<IActionResult> GetAllReviews()
    {
        try
        {
            var reviews = await _reviewRepository.GetAllReviewsAsync();
            var reviewDTOs = _mapper.Map<IEnumerable<ReviewDTO>>(reviews); // Map to DTO
            return Ok(reviewDTOs);
        }
        catch (InternalServerException ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    // GET: api/Reviews/{reviewId}
    [HttpGet("{reviewId}")]
    [Authorize(Roles = "User, Admin")] // Allow both User and Admin to get a specific review
    public async Task<IActionResult> GetReviewById(int reviewId)
    {
        try
        {
            var review = await _reviewRepository.GetReviewByIdAsync(reviewId);
            if (review == null)
            {
                throw new NotFoundException("Review not found");
            }
            var reviewDTO = _mapper.Map<ReviewDTO>(review); // Map to DTO
            return Ok(reviewDTO);
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

    // POST: api/Reviews
    [HttpPost]
    [Authorize(Roles = "User")] // Only Users can add reviews
    public async Task<IActionResult> AddReview([FromBody] ReviewDTO reviewDTO)
    {
        try
        {
            var review = _mapper.Map<Review>(reviewDTO); // Map from DTO to model
            await _reviewRepository.AddReviewAsync(review);
            var createdReviewDTO = _mapper.Map<ReviewDTO>(review); // Map to DTO for response
            return CreatedAtAction(nameof(GetReviewById), new { reviewId = review.ReviewId }, createdReviewDTO);
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

    // PUT: api/Reviews
    [HttpPut]
    [Authorize(Roles = "User, Admin")] // Allow Users to update their own reviews and Admin to update any review
    public async Task<IActionResult> UpdateReview([FromBody] ReviewDTO reviewDTO)
    {
        try
        {
            var review = _mapper.Map<Review>(reviewDTO); // Map from DTO to model
            await _reviewRepository.UpdateReviewAsync(review);
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

    // DELETE: api/Reviews/{reviewId}
    [HttpDelete("{reviewId}")]
    [Authorize(Roles = "User, Admin")] // Allow Users to delete their own reviews and Admin to delete any review
    public async Task<IActionResult> DeleteReview(int reviewId)
    {
        try
        {
            await _reviewRepository.DeleteReviewAsync(reviewId);
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
}
