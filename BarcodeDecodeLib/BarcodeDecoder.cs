using OpenCvSharp;
using ZXing;

namespace BarcodeDecodeLib;

public class BarcodeDecoder
{
    public void ScanWithDifferentTechs(string path)
    {
        ScanWithOpenCV(path);
    }

    public void ScanSerializedWithDifferentTechs(byte[] imageData)
    {
        ScanSerializedWithOpenCV(imageData);
    }

    private Result? ScanWithOpenCV(string path)
    {
        var options = new ZXing.Common.DecodingOptions
        {
            TryHarder = true,
            PossibleFormats = new List<BarcodeFormat> { BarcodeFormat.CODE_128, BarcodeFormat.CODE_39 }
        };
        var barcodeReader = new ZXing.OpenCV.BarcodeReader
        {
            Options = options
        };
        Mat image = Cv2.ImRead(path, ImreadModes.Color);

        Mat grayImage = new Mat();

        Cv2.CvtColor(image, grayImage, ColorConversionCodes.BGR2GRAY);


        var barcodeResult = barcodeReader.Decode(grayImage);
        Cv2.ImWrite("corrected_image_decode.jpg", image);

        Console.WriteLine($"Decoded barcode text: {barcodeResult?.Text}");
        Console.WriteLine($"Barcode format: {barcodeResult?.BarcodeFormat}");
        return barcodeResult;
    }

    private Result? ScanSerializedWithOpenCV(byte[] imageData)
    {

        Mat image = Mat.FromImageData(imageData, ImreadModes.Color);

        var options = new ZXing.Common.DecodingOptions
        {
            TryHarder = true
        };

        var barcodeReader = new ZXing.OpenCV.BarcodeReader
        {
            Options = options
        };


        var barcodeResult = barcodeReader.Decode(image);
        
        Console.WriteLine($"[Serialized] Decoded barcode text: {barcodeResult?.Text}");
        Console.WriteLine($"[Serialized] Barcode format: {barcodeResult?.BarcodeFormat}");
        return barcodeResult;
    }
}