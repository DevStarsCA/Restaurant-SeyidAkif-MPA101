using QRCoder;
using Restaurant.Application.Common.Interfaces;

public class QRCodeService : IQRCodeService
{
    public string GenerateQRCode(string data)
    {
        using var qrGenerator = new QRCodeGenerator();
        var qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new Base64QRCode(qrCodeData);
        return qrCode.GetGraphic(10);
    }

    public byte[] GenerateQRCodeImage(string data)
    {
        using var qrGenerator = new QRCodeGenerator();
        var qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrCodeData);
        return qrCode.GetGraphic(10);
    }
}