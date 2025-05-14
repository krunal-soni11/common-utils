namespace Services.Auth;

public class ValidateJwtResponseDto
{
    public string UserId { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new List<string>();
    public bool IsValid { get; set; } = false;
    public string ErrorMessage { get; set; } = string.Empty;
}