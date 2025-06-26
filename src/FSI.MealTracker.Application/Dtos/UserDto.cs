namespace FSI.MealTracker.Application.Dtos
{
    public class UserDto : BaseDto
    {
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
    }
}
