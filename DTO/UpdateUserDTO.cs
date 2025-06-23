namespace Eagles_Portal.DTO
{
    public class UpdateUserDTO
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? OtherName { get; set; }
        public string Email { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
    }
}
