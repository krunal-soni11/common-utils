namespace Services.Auth;
public interface IAuthService
{
    CreateJwtResponseDto CreateJWT(CreateJwtRequestDto createJwtRequestDto);
    Task<ValidateJwtResponseDto> ValidateJWT(ValidateJwtRequestDto validateJwtRequestDto);
}