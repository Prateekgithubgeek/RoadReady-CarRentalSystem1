using CarRental.Models.DTOs;
using System.Threading.Tasks;

namespace CarRental.Repository
{
    public interface IPasswordResetRepository
    {
        Task<IEnumerable<PasswordResetDTO>> GetAllPasswordResetsAsync();
        Task<PasswordResetDTO> GetPasswordResetByIdAsync(int id);
        Task<PasswordResetDTO> AddPasswordResetAsync(PasswordResetDTO passwordResetDto);
        Task UpdatePasswordResetAsync(int id, PasswordResetDTO passwordResetDto);
        Task DeletePasswordResetAsync(int id);
    }
}
