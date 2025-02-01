using OpenCvSharp;

namespace BarcodeDecodeFrontend.Data.Services;

public class VideoProcessor
{
    public List<Mat> GetVideoFrames(string tempFilePath, int frameStep = 1, string fileExtension = ".mp4")
    {
        List<Mat> frames = new List<Mat>();

        try
        {
            using (VideoCapture capture = new VideoCapture(tempFilePath))
            {
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