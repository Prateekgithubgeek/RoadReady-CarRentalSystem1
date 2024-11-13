using CarRental.Models;
using CarRental.Models.DTOs;
using CarRental.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarRental.Repository
{
    public class PasswordResetRepository : IPasswordResetRepository
    {
        private readonly YourDbContext _context;

        public PasswordResetRepository(YourDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PasswordResetDTO>> GetAllPasswordResetsAsync()
        {
            // Fetch all password resets and map to DTOs
            var passwordResets = await _context.PasswordResets
                .Select(pr => new PasswordResetDTO
                {
                    ResetId = pr.ResetId,
                    UserId = pr.UserId,
                    ResetToken = pr.ResetToken,
                    ExpirationDate = pr.ExpirationDate,
                    IsUsed = pr.IsUsed,
                    UserEmail = pr.User.Email
                })
                .ToListAsync();

            return passwordResets;
        }

        public async Task<PasswordResetDTO> GetPasswordResetByIdAsync(int id)
        {
            // Fetch password reset by ID and map to DTO
            var passwordReset = await _context.PasswordResets
                .Where(pr => pr.ResetId == id)
                .Select(pr => new PasswordResetDTO
                {
                    ResetId = pr.ResetId,
                    UserId = pr.UserId,
                    ResetToken = pr.ResetToken,
                    ExpirationDate = pr.ExpirationDate,
                    IsUsed = pr.IsUsed,
                    UserEmail = pr.User.Email
                })
                .FirstOrDefaultAsync();

            if (passwordReset == null)
            {
                throw new NotFoundException($"Password reset with ID {id} not found.");
            }

            return passwordReset;
        }

        public async Task<PasswordResetDTO> AddPasswordResetAsync(PasswordResetDTO passwordResetDto)
        {
            // Check for duplicate token
            var existingReset = await _context.PasswordResets
                .AnyAsync(pr => pr.ResetToken == passwordResetDto.ResetToken);

            if (existingReset)
            {
                throw new DuplicateResourceException("Password reset with the same token already exists.");
            }

            // Map DTO to entity
            var passwordReset = new PasswordReset
            {
                UserId = passwordResetDto.UserId,
                ResetToken = passwordResetDto.ResetToken,
                ExpirationDate = passwordResetDto.ExpirationDate,
                IsUsed = passwordResetDto.IsUsed
            };

            // Add new password reset
            _context.PasswordResets.Add(passwordReset);
            await _context.SaveChangesAsync();

            // Map the entity back to DTO for return
            return new PasswordResetDTO
            {
                ResetId = passwordReset.ResetId,
                UserId = passwordReset.UserId,
                ResetToken = passwordReset.ResetToken,
                ExpirationDate = passwordReset.ExpirationDate,
                IsUsed = passwordReset.IsUsed,
                UserEmail = passwordReset.User.Email
            };
        }

        public async Task UpdatePasswordResetAsync(int id, PasswordResetDTO passwordResetDto)
        {
            var passwordReset = await _context.PasswordResets.FindAsync(id);

            if (passwordReset == null)
            {
                throw new NotFoundException($"Password reset with ID {id} not found.");
            }

            // Update the properties of the entity from the DTO
            passwordReset.ResetToken = passwordResetDto.ResetToken;
            passwordReset.ExpirationDate = passwordResetDto.ExpirationDate;
            passwordReset.IsUsed = passwordResetDto.IsUsed;

            _context.Entry(passwordReset).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeletePasswordResetAsync(int id)
        {
            var passwordReset = await _context.PasswordResets.FindAsync(id);

            if (passwordReset == null)
            {
                throw new NotFoundException($"Password reset with ID {id} not found.");
            }

            _context.PasswordResets.Remove(passwordReset);
            await _context.SaveChangesAsync();
        }
    }
}
