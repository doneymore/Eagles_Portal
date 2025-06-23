using Eagles_Portal.DTO;
using Eagles_Portal.Models;

namespace Eagles_Portal.contracts.Interface
{
    public interface IUserServices
    {
        Task<IEnumerable<Users>> GetAllUsersAsync();
        Task<Users> GetUserByIdAsync(int id);
        Task AddUserAsync(Users user);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> UpdateUserPartialAsync(int id, UserResponseDTOcs users);
    }
}
