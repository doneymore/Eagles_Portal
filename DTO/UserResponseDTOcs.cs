namespace Eagles_Portal.DTO
{
    public class UserResponseDTOcs
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? OtherName { get; set; }
        public string Email { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
