public class HelperPack
{
    public const string bugReportFormUrl = @"https://forms.office.com/r/yesm6nsfsM?origin=lprLink";
    const string commentKey = "LOTR|ABCD|abcd|1234|~!@#";
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

    public static string DecryptText(string encryptText)
    {
        byte[] results;
        using (SHA256CryptoServiceProvider hashProvider = new())
        {
            using TripleDESCryptoServiceProvider tdesAlgorithm = new();
            byte[] dataToDecrypt = Convert.FromBase64String(encryptText);
            byte[] tdesKey = hashProvider.ComputeHash(Encoding.UTF8.GetBytes(commentKey));
            byte[] trimmedBytes = new byte[24];
            Buffer.BlockCopy(tdesKey, 0, trimmedBytes, 0, 24);
            tdesAlgorithm.Key = trimmedBytes;
            tdesAlgorithm.Mode = CipherMode.ECB;
            tdesAlgorithm.Padding = PaddingMode.PKCS7;
            try
            {
                results = tdesAlgorithm.CreateDecryptor().TransformFinalBlock(dataToDecrypt, 0, dataToDecrypt.Length);
            }
            finally
            {
                tdesAlgorithm.Clear();
                hashProvider.Clear();
            }
        }
        return Encoding.UTF8.GetString(results);
    }

    public static string EncryptText(string plainText)
    {
        if (string.IsNullOrWhiteSpace(plainText))
            return string.Empty;

        byte[] results;
        using (SHA256CryptoServiceProvider hashProvider = new())
        {
            using TripleDESCryptoServiceProvider tdesAlgorithm = new();
            byte[] tdesKey = hashProvider.ComputeHash(Encoding.UTF8.GetBytes(commentKey));
            var trimmedBytes = new byte[24];
            Buffer.BlockCopy(tdesKey, 0, trimmedBytes, 0, 24);
            tdesAlgorithm.Key = trimmedBytes;
            tdesAlgorithm.Mode = CipherMode.ECB;
            tdesAlgorithm.Padding = PaddingMode.PKCS7;
            byte[] dataToEncrypt = Encoding.UTF8.GetBytes(plainText);
            try
            {
                results = tdesAlgorithm.CreateEncryptor().TransformFinalBlock(dataToEncrypt, 0, dataToEncrypt.Length);
            }
            finally
            {
                tdesAlgorithm.Clear();
                hashProvider.Clear();
            }
        }
        return Convert.ToBase64String(results);
    }
}