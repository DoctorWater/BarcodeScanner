using OpenCvSharp;
using SixLabors.ImageSharp.PixelFormats;
using ZXing;

namespace BarcodeScanner.App;

class BarcodeDecoder
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
            TryHarder = true, // Попробовать другие способы декодирования
            PossibleFormats = new List<BarcodeFormat> { BarcodeFormat.CODE_128, BarcodeFormat.CODE_39 }
        };
        // create a barcode reader instance
        var barcodeReader = new ZXing.OpenCV.BarcodeReader
        {
            Options = options
        };

        // create an in memory bitmap
        Mat image = Cv2.ImRead(path, ImreadModes.Color);

        Mat grayImage = new Mat();

        Cv2.CvtColor(image, grayImage, ColorConversionCodes.BGR2GRAY);

        // decode the barcode from the in memory bitmap
        var barcodeResult = barcodeReader.Decode(grayImage);
        Cv2.ImWrite("corrected_image_decode.jpg", image);
        // output results to console
        Console.WriteLine($"Decoded barcode text: {barcodeResult?.Text}");
        Console.WriteLine($"Barcode format: {barcodeResult?.BarcodeFormat}");
        return barcodeResult;
    }

    private Result? ScanSerializedWithOpenCV(byte[] imageData)
    {
        // Декодируем массив байтов в Mat
        Mat image = Mat.FromImageData(imageData, ImreadModes.Color);

        var options = new ZXing.Common.DecodingOptions
        {
            TryHarder = true, // Попробовать другие способы декодирования
            PossibleFormats = new List<BarcodeFormat> { BarcodeFormat.CODE_128, BarcodeFormat.CODE_39 }
        };
        // create a barcode reader instance
        var barcodeReader = new ZXing.OpenCV.BarcodeReader
        {
            Options = options
        };

        // decode the barcode from the in memory bitmap
        var barcodeResult = barcodeReader.Decode(image);

        // output results to console
        Console.WriteLine($"[Serialized] Decoded barcode text: {barcodeResult?.Text}");
        Console.WriteLine($"[Serialized] Barcode format: {barcodeResult?.BarcodeFormat}");
        return barcodeResult;
    }
}