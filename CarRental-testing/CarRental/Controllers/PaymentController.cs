using AutoMapper;

using CarRental.DTOs;
using CarRental.Exceptions;
using CarRental.Models;
using CarRental.Models.DTOs;
using CarRental.Repository;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
    
using System.ComponentModel.DataAnnotations;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentController : ControllerBase
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IMapper _mapper;

    public PaymentController(IPaymentRepository paymentRepository, IMapper mapper)
    {
        _paymentRepository = paymentRepository;
        _mapper = mapper;
    }

    // GET: api/Payments
    [HttpGet]
    [Authorize(Roles = "User, Admin")]
    public async Task<IActionResult> GetAllPayments()
    {
        var payments = await _paymentRepository.GetAllPaymentsAsync();
        var paymentDTOs = _mapper.Map<IEnumerable<PaymentDTO>>(payments);
        return Ok(paymentDTOs);
    }

    // GET: api/Payments/{paymentId}
    [HttpGet("{paymentId}")]
    [Authorize(Roles = "User, Admin")]
    public async Task<IActionResult> GetPaymentById(int paymentId)
    {
        var payment = await _paymentRepository.GetPaymentByIdAsync(paymentId);
        if (payment == null)
        {
            return NotFound("Payment not found.");
        }

        var paymentDTO = _mapper.Map<PaymentDTO>(payment);
        return Ok(paymentDTO);
    }

    // POST: api/Payments
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddPayment([FromBody] CreatePaymentDTO createPaymentDTO)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var payment = _mapper.Map<Payment>(createPaymentDTO);
        await _paymentRepository.AddPaymentAsync(payment);
        var createdPaymentDTO = _mapper.Map<PaymentDTO>(payment);

        return CreatedAtAction(nameof(GetPaymentById), new { paymentId = payment.PaymentId }, createdPaymentDTO);
    }

    // PUT: api/Payments/{paymentId}
    [HttpPut("{paymentId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdatePayment(int paymentId, [FromBody] PaymentDTO paymentDTO)
    {
        if (paymentId != paymentDTO.PaymentId)
        {
            return BadRequest("Payment ID mismatch.");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var payment = _mapper.Map<Payment>(paymentDTO);
        await _paymentRepository.UpdatePaymentAsync(payment);
        return NoContent();
    }

    // DELETE: api/Payments/{paymentId}
    [HttpDelete("{paymentId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeletePayment(int paymentId)
    {
        await _paymentRepository.DeletePaymentAsync(paymentId);
        return NoContent();
    }
}
