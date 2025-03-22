using OpenCvSharp;

namespace BarcodeDecodeFrontend.Data.Services.Interfaces;

public interface IBarcodeDecoder
{
    string? Decode(byte[] imageData);
    string? Decode(Mat image);
}