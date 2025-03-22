using OpenCvSharp;

namespace BarcodeDecodeFrontend.Data.Services.Interfaces;

public interface IVideoProcessor
{
    List<Mat> GetVideoFrames(string tempFilePath, string fileExtension = ".mp4");
}