namespace Restaurant.Application.Common.Interfaces;

public interface IQRCodeService
{
    string GenerateQRCode(string data);
    byte[] GenerateQRCodeImage(string data);
}







