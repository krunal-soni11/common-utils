public class HelperPack
{
    public const string bugReportFormUrl = @"https://forms.office.com/r/yesm6nsfsM?origin=lprLink";
    private const string encryptionKey = "LOTR|ABCD|LogiTrack|1234|~!@#";
    public static string Decrypt(string encryptedString)
    {
        string decryptedString = string.Empty;
        
        using (SHA256 sHA256 = SHA256.Create())
        {
            byte[] dataToDecrypt = Convert.FromBase64String(encryptedString);
            byte[] computedHashCode = sHA256.ComputeHash(Encoding.UTF8.GetBytes(encryptionKey));
            byte[] aesKey = new byte[32];
            Buffer.BlockCopy(computedHashCode, 0, aesKey, 0, aesKey.Length);

            using (Aes aes = Aes.Create())
            {
                aes.Key = aesKey;
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;
                ICryptoTransform symmetricDecryptor = aes.CreateDecryptor();
                byte[] decryptedDataBytes = symmetricDecryptor.TransformFinalBlock(dataToDecrypt, 0, dataToDecrypt.Length);
                decryptedString = Encoding.UTF8.GetString(decryptedDataBytes);
            }
        }
        return decryptedString;
    }

    public static string Encrypt(string plainString)
    {
        string encryptedString = string.Empty;
        if (!string.IsNullOrWhiteSpace(plainString))
        {
            using (SHA256 sHA256 = SHA256.Create())
            {
                byte[] encryptionKeyBytes = Encoding.UTF8.GetBytes(encryptionKey);
                byte[] computedHashCode = sHA256.ComputeHash(encryptionKeyBytes);
                var aesKey = new byte[32];
                Buffer.BlockCopy(computedHashCode, 0, aesKey, 0, aesKey.Length);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = aesKey;
                    aes.Mode = CipherMode.ECB;
                    aes.Padding = PaddingMode.PKCS7;
                    byte[] dataToEncrypt = Encoding.UTF8.GetBytes(plainString);
                    ICryptoTransform symmetricEncryptor = aes.CreateEncryptor();
                    byte[] encryptedDataBytes = symmetricEncryptor.TransformFinalBlock(dataToEncrypt, 0, dataToEncrypt.Length);
                    encryptedString = Convert.ToBase64String(encryptedDataBytes);
                }
            }
        }
        return encryptedString;
    }

    public static byte[] GenerateCodeImage(string content, CodeType codeType = CodeType.QR, int width = 300, int height = 100, int margin = 1)
    {
        var writer = new BarcodeWriter<SKBitmap>
        {
            Format = GetBarcodeFormat(codeType),
            Options = new EncodingOptions
            {
                Width = width,
                Height = height,
                Margin = margin
            },
            Renderer = new SKBitmapRenderer()
        };

        using (var skBitmap = writer.Write(content))
        using (var skImage = SKImage.FromBitmap(skBitmap))
        using (var data = skImage.Encode(SKEncodedImageFormat.Png, 100))
        {
            return data.ToArray();
        }
    }

    private static BarcodeFormat GetBarcodeFormat(CodeType codeType) => codeType switch
    {
        CodeType.QR => BarcodeFormat.QR_CODE,
        CodeType.Barcode => BarcodeFormat.CODE_128,
        CodeType.Aztec => BarcodeFormat.AZTEC,
        CodeType.DataMatrix => BarcodeFormat.DATA_MATRIX,
        CodeType.PDF417 => BarcodeFormat.PDF_417,
        _ => throw new ArgumentOutOfRangeException(nameof(codeType), codeType, null)
    };
}