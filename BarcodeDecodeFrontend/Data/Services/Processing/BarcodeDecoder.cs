using BarcodeDecodeFrontend.Data.Services.Interfaces;
using OpenCvSharp;
using ZXing;

namespace BarcodeDecodeFrontend.Data.Services.Processing;

public class BarcodeDecoder : IBarcodeDecoder
{

    public string? Decode(byte[] imageData)
    {
        return ScanWithOpenCV(imageData)?.Text;
    }
    
    public string? Decode(Mat image)
    {
        return ScanWithOpenCV(image)?.Text;
    }
    
    private Result? ScanWithOpenCV(byte[] imageData)
    {

        Mat image = Mat.FromImageData(imageData, ImreadModes.Color);

        return ScanWithOpenCV(image);
    }
    
    private Result? ScanWithOpenCV(Mat image)
    {

        var options = new ZXing.Common.DecodingOptions
        {
            TryHarder = true
        };

        var barcodeReader = new ZXing.OpenCV.BarcodeReader
        {
            Options = options
        };


        var barcodeResult = barcodeReader.Decode(image);

        return barcodeResult;
    }
}