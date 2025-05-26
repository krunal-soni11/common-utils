namespace Common.Utils.Tests;

public class HelperPackTests
{
    [Theory]
    [InlineData("iposapp", CodeType.QR)]
    [InlineData("123456", CodeType.Barcode)]
    public void GenerateCodeImage_ShouldReturn_ValidPngBytes(string content, CodeType codeType)
    {
        // Act
        var result = HelperPack.GenerateCodeImage(content, codeType);

        // Assert
        Assert.NotNull(result);

        // PNG files always start with this 8-byte signature:
        var pngSignature = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 };
        for (int i = 0; i < pngSignature.Length; i++)
        {
            Assert.Equal(pngSignature[i], result[i]);
        }
    }
    [Theory]
    [InlineData("ipos", "auth", "token")]
    public void CreateCookieName_ShouldReturnName_InValidConvention(string appName, string module, string purpose)
    {
        var result = HelperPack.CreateCookieName(appName, module, purpose);

        Assert.NotNull(result);

        Assert.Equal("ipos_auth_token_dev", result);
    }
}
