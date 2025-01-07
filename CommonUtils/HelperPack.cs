public class HelperPack
{
    public const string bugReportFormUrl = @"https://forms.office.com/r/yesm6nsfsM?origin=lprLink";
    private const string encryptionKey = "LOTR|ABCD|LogiTrack|1234|~!@#";

    public static byte[] GenerateQRCode(string qrText)
    {
        using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
        {
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q);
            using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
            {
                byte[] qrCodeAsPng = qrCode.GetGraphic(20);
                return qrCodeAsPng;
            }
        }
    }

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
}