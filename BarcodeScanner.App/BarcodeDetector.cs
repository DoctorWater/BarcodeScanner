using OpenCvSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Tesseract;
using Rect = Tesseract.Rect;
using Rectangle = System.Drawing.Rectangle;
using Size = SixLabors.ImageSharp.Size;

namespace BarcodeScanner.App;

public class BarcodeDetector
{
    public void DetectBarcode(string path)
    {
        var tessDataPath = Path.Combine(Directory.GetCurrentDirectory(), "tessdata");
        try
        {
            // Инициализация OCR
            using (var engine = new TesseractEngine(tessDataPath, "eng+rus", EngineMode.Default))
            {
                using (var img = Pix.LoadFromFile(path))
                {
                    // Распознавание текста
                    var page = engine.Process(img);

                    // Получение всех сегментов текста
                    var regions = page.GetSegmentedRegions(PageIteratorLevel.TextLine);
                    List<Rectangle> textRegions = new List<Rectangle>();

                    // Собираем все координаты текста
                    foreach (var region in regions)
                    {
                        textRegions.Add(region);
                    }

                    // Объединяем области в одну
                    var boundingBox = MergeBoundingBoxes(textRegions);

                    // Выводим объединенную область
                    Console.WriteLine(
                        $"Объединенная область: X={boundingBox.X}, Y={boundingBox.Y}, Width={boundingBox.Width}, Height={boundingBox.Height}");
                    DrawRectangle(path, boundingBox);
                    var correctedImage = CorrectPerspective(path, boundingBox);

                    // Сохраняем результат
                    /*correctedImage.SaveImage("corrected_image.jpg");
                    Console.WriteLine("Сохранено выпрямленное изображение: corrected_image.jpg");*/
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }

    static void DrawRectangle(string imagePath, Rectangle boundingBox)
    {
        // Загрузка изображения с помощью ImageSharp
        using (var image = Image.Load<Rgba32>(imagePath))
        {
            // Рисуем прямоугольник вокруг текста
            image.Mutate<Rgba32>(x => x.DrawPolygon(Color.Red, 2.0f, new PointF[]
            {
                new PointF(boundingBox.X, boundingBox.Y),
                new PointF(boundingBox.X + boundingBox.Width, boundingBox.Y),
                new PointF(boundingBox.X + boundingBox.Width, boundingBox.Y + boundingBox.Height),
                new PointF(boundingBox.X, boundingBox.Y + boundingBox.Height),
            }));

            // Сохраняем результат с выделенными границами
            string outputPath = "output_with_borders.jpg";
            image.Save(outputPath);
            Console.WriteLine($"Сохранено изображение с границами: {outputPath}");
        }
    }

    static Rectangle MergeBoundingBoxes(List<Rectangle> regions)
    {
        int x1 = int.MaxValue, y1 = int.MaxValue, x2 = int.MinValue, y2 = int.MinValue;

        foreach (var region in regions)
        {
            x1 = Math.Min(x1, region.X);
            y1 = Math.Min(y1, region.Y);
            x2 = Math.Max(x2, region.X + region.Width);
            y2 = Math.Max(y2, region.Y + region.Height);
        }

        return new Rectangle(x1, y1, x2 - x1, y2 - y1);
    }

    static Mat CorrectPerspective(string imagePath, Rectangle boundingBox)
    {
        // Загружаем изображение
        Mat mat = Cv2.ImRead(imagePath);

        // Получаем координаты углов прямоугольника
        Point2f[] points = new Point2f[]
        {
            new Point2f(boundingBox.X, boundingBox.Y),
            new Point2f(boundingBox.X + boundingBox.Width, boundingBox.Y),
            new Point2f(boundingBox.X + boundingBox.Width, boundingBox.Y + boundingBox.Height),
            new Point2f(boundingBox.X, boundingBox.Y + boundingBox.Height)
        };

        // Вычисляем новые координаты для выпрямленного изображения
        Point2f[] destPoints = new Point2f[]
        {
            new Point2f(0, 0),
            new Point2f(mat.Width, 0),
            new Point2f(mat.Width, mat.Height),
            new Point2f(0, mat.Height)
        };

        // Выполняем преобразование перспективы
        Mat perspectiveMatrix = Cv2.GetPerspectiveTransform(points, destPoints);
        Mat result = new Mat();
        Cv2.WarpPerspective(mat, result, perspectiveMatrix, new OpenCvSharp.Size(mat.Width, mat.Height));
        
        result.SaveImage("corrected_image.jpg");
        // Возвращаем выпрямленное изображение
        return result;
    }
}