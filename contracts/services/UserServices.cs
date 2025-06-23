using Eagles_Portal.contracts.Interface;
using Eagles_Portal.DTO;
using Eagles_Portal.Models;
using Microsoft.EntityFrameworkCore;

namespace Eagles_Portal.contracts.services
{
    public class UserServices : IUserServices
    {
        private readonly ApplicationDbContext _context;

        public UserServices(ApplicationDbContext context)
        {
            _context = context;
        }



        public async Task<IEnumerable<Users>> GetAllUsersAsync() =>
            await _context.Users.ToListAsync();

        public async Task<Users> GetUserByIdAsync(int id) =>
            await _context.Users.FindAsync(id);

        public async Task AddUserAsync(Users user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateUserPartialAsync(int id, UserResponseDTOcs userDto) // Renamed parameter to userDto for clarity
        {
            var userToUpdate = await _context.Users.FindAsync(id); // Get the entity from the database
            if (userToUpdate == null)
                return false;

            // Apply updates only if the DTO field is not null or whitespace
            if (!string.IsNullOrWhiteSpace(userDto.FirstName))
                userToUpdate.FirstName = userDto.FirstName; // Correct: Assign DTO value to entity

            if (!string.IsNullOrWhiteSpace(userDto.LastName))
                userToUpdate.LastName = userDto.LastName; // Correct: Assign DTO value to entity

            if (!string.IsNullOrWhiteSpace(userDto.OtherName))
                userToUpdate.OtherName = userDto.OtherName; // Correct: Assign DTO value to entity

            if (!string.IsNullOrWhiteSpace(userDto.Email))
                userToUpdate.Email = userDto.Email; // Correct: Assign DTO value to entity

            // You might need to add similar checks for other properties in your DTO

            await _context.SaveChangesAsync(); // Save the changes to the database
            return true;
        }


    }
}
