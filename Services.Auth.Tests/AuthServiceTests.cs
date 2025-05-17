namespace Services.Auth.Tests;

public class AuthServiceTests
{
    private readonly JwtSettings jwtSettings = new JwtSettings
    {
        Key = "using_IPOS_shopping_now_hassle_free_24*7",
        Issuer = "testIssuer",
        Audience = "testAudience"
    };
    private readonly Mock<IOptions<JwtSettings>> mockOptions;
    private readonly IAuthService _authService;

    public AuthServiceTests()
    {
        mockOptions = new Mock<IOptions<JwtSettings>>();
        mockOptions.Setup(x => x.Value).Returns(jwtSettings);
        _authService = new AuthService(mockOptions.Object);
    }

    [Fact(DisplayName = "Given valid request, when CreateJWT is called, then valid token returned")]
    public void CreateJWT_ShouldCreateAndReturnTokenResponse()
    {
        var requestDto = new CreateJwtRequestDto
        {
            UserId = "user-123",
            Username = "testuser",
            Roles = new List<string> { "Admin", "User" }
        };

        var result = _authService.CreateJWT(requestDto);

        Assert.NotNull(result);
        Assert.False(string.IsNullOrWhiteSpace(result.AccessToken));
        Assert.True(result.ExpiresAt > DateTime.UtcNow);

        var handler = new JsonWebTokenHandler();
        var jwt = handler.ReadJsonWebToken(result.AccessToken);
        Assert.Contains(jwt.Claims, c => c.Type == "nameid" && c.Value == "user-123");
        Assert.Contains(jwt.Claims, c => c.Type == "name" && c.Value == "testuser");
        //Assert.Contains(jwt.Claims, c => c.Type == "roles" && c.Value == "Admin");
        //Assert.Contains(jwt.Claims, c => c.Type == "roles" && c.Value == "User");
    }

    [Fact(DisplayName = "Given empty accesstoken, when ValidateJWT is called, then exception is thrown")]
    public async Task ValidateJWT_ShouldValidateAndReturnTokenResponse_ThrowsException()
    {
        var requestDto = new ValidateJwtRequestDto
        {
            AccessToken = "",
            SecretKey = jwtSettings.Key
        };

        var result = await _authService.ValidateJWT(requestDto);
        //var result = await Assert.ThrowsAsync<ArgumentNullException>(() => _authService.ValidateJWT(requestDto));
        Assert.NotNull(result.ErrorMessage);
        Assert.True(result.IsValid == false);

    }
    [Fact(DisplayName = "Given expired access token, when ValidateJWT is called, then exception thrown")]
    public async Task ValidateJWT_ShouldValidateAndReturnTokenResponse()
    {
        var requestDto = new ValidateJwtRequestDto
        {
            AccessToken = "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9.eyJhdWQiOiJ0ZXN0QXVkaWVuY2UiLCJpc3MiOiJ0ZXN0SXNzdWVyIiwiZXhwIjoxNzQ3NDgyODM1LCJpYXQiOjE3NDc0NzkyMzUsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWVpZGVudGlmaWVyIjoidXNlci0xMjMiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoidGVzdHVzZXIiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOlsiQWRtaW4iLCJVc2VyIl0sIm5iZiI6MTc0NzQ3OTMwN30.-6hwQMDDSwI8M4XJYQ4vK6eF2CuOeQG7BAnKwjs_PcU",
            SecretKey = jwtSettings.Key
        };
        var result = await _authService.ValidateJWT(requestDto);
        Assert.True(result.IsValid == false);
        Assert.NotNull(result.ErrorMessage);
    }
    [Fact(DisplayName = "Given valid access token, when ValidateJWT is called, then valid token returned")]
    public async Task ValidateJWT_ShouldValidateAndReturnValidTokenResponse()
    {
        var requestDto = new ValidateJwtRequestDto
        {
            AccessToken = "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9.eyJhdWQiOiJ0ZXN0QXVkaWVuY2UiLCJpc3MiOiJ0ZXN0SXNzdWVyIiwiZXhwIjoxNzQ3NDkyNDQ1LCJpYXQiOjE3NDc0ODg4NDUsInJvbGVzIjoiW1wiQWRtaW5cIixcIlVzZXJcIl0iLCJuYW1laWQiOiJ1c2VyLTEyMyIsIm5hbWUiOiJ0ZXN0dXNlciIsIm5iZiI6MTc0NzQ4ODg0OH0.1HiOmN5C2Lv9JCnAb0SDBvzLb2F6hEgJI_l4jXlgGmE",
            SecretKey = jwtSettings.Key
        };
        var result = await _authService.ValidateJWT(requestDto);
        Assert.True(result.IsValid == true);
    }
}