using BarcodeDecodeFrontend.Data.Services.Interfaces;
using OpenCvSharp;

namespace BarcodeDecodeFrontend.Data.Services.Processing;

public class VideoProcessor : IVideoProcessor
{
    private const int OVERALL_FRAME_COUNT = 20;
    public List<Mat> GetVideoFrames(string tempFilePath, string fileExtension = ".mp4")
    {
        List<Mat> frames = new List<Mat>();

        try
        {
            using (VideoCapture capture = new VideoCapture(tempFilePath))
            {
                var frameStep = capture.FrameCount / OVERALL_FRAME_COUNT;
                if (!capture.IsOpened())
                {
                    Console.WriteLine("Ошибка: не удалось открыть видеофайл из сериализованных данных.");
                    return frames;
                }

                int frameIndex = 0;
                Mat frame = new Mat();

                while (true)
                {
                    bool success = capture.Read(frame);
                    if (!success || frame.Empty())
                        break;

                    // Если номер кадра кратен frameStep, сохраняем его
                    if (frameIndex % frameStep == 0)
                    {
                        frames.Add(frame.Clone());
                    }
                    frameIndex++;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при обработке видео: {ex.Message}");
        }
        

        return frames;
    }
}