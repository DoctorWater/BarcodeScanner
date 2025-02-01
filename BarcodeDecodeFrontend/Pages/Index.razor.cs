using System.Text;
using BarcodeDecodeFrontend.Shared.Modals;
using BarcodeDecodeLib.Models.Messages;
using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace BarcodeDecodeFrontend.Pages;

public partial class Index
{
    [CascadingParameter] public IModalService Modal { get; set; } = default!;
    
    private List<IBrowserFile> loadedFiles = new();
    private byte[] imageBytes;
    private List<string> _recognizedBarcodes = new();

    private async Task LoadPhotoFiles(InputFileChangeEventArgs e)
    {
        var files = e.GetMultipleFiles();
        var fileVerifyResult = VerifyFiles(files, "image");
        var userMessage = CreateUserMessage(fileVerifyResult);
        if (userMessage != string.Empty)
        {
            var parameters = new ModalParameters();
            parameters.Add(nameof(ModalAlert.Message), userMessage);

            Modal.Show<ModalAlert>("ALERT", parameters);
        }

        var imageFiles = fileVerifyResult.Where(p => p.Value is true).Select(x => x.Key).ToList();
        await UploadImageFiles(imageFiles);
    }
    
    private async Task LoadVideoFiles(InputFileChangeEventArgs e)
    {
        var files = e.GetMultipleFiles();
        var fileVerifyResult = VerifyFiles(files, "video");
        var userMessage = CreateUserMessage(fileVerifyResult);
        if (userMessage != string.Empty)
        {
            var parameters = new ModalParameters();
            parameters.Add(nameof(ModalAlert.Message), userMessage);

            Modal.Show<ModalAlert>("ALERT", parameters);
        }

        var videoFiles = fileVerifyResult.Where(p => p.Value is true).Select(x => x.Key).ToList();
        await UploadVideoFiles(videoFiles);
    }

    private Dictionary<IBrowserFile, bool> VerifyFiles(IReadOnlyList<IBrowserFile> files, string contentType)
    {
        Dictionary<IBrowserFile, bool> fileVerifyResult = new Dictionary<IBrowserFile, bool>();
        foreach (var file in files)
        {
            fileVerifyResult.Add(file, file.ContentType.Contains(contentType));
        }

        return fileVerifyResult;
    }

    private string CreateUserMessage(Dictionary<IBrowserFile, bool> fileVerifyResult)
    {
        var sb = new StringBuilder();
        foreach (var pair in fileVerifyResult)
        {
            if (pair.Value is false)
            {
                sb.AppendLine($"File {pair.Key.Name} is invalid");
            }
        }

        return sb.ToString();
    }

    private async Task UploadImageFiles(IEnumerable<IBrowserFile> files)
    {
        loadedFiles.Clear();
        foreach (var file in files)
        {
            try
            {
                await using MemoryStream ms = new MemoryStream();
                await file.OpenReadStream().CopyToAsync(ms);
                _recognizedBarcodes.Add(Decoder.Decode(ms.ToArray()) ?? String.Empty);
            }
            catch (Exception ex)
            {
            }
        }
    }
    
    private async Task UploadVideoFiles(IEnumerable<IBrowserFile> files)
    {
        loadedFiles.Clear();

        foreach (var file in files)
        {
            string tempFolder = Path.GetTempPath();
            string extension = Path.GetExtension(file.Name);
            string tempFileName = Path.Combine(tempFolder, $"{Guid.NewGuid()}{extension}");
            try
            {
                await using (var fileStream = File.Create(tempFileName))
                {
                    await using (var stream = file.OpenReadStream(10485760L))
                    {
                        await stream.CopyToAsync(fileStream);
                    }
                }
                
                var frames =VideoProcessor.GetVideoFrames(tempFileName, 4, extension);
                foreach (var frame in frames)
                {
                    _recognizedBarcodes.Add(Decoder.Decode(frame) ?? String.Empty);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки файла {file.Name}: {ex.Message}");
            }
            finally
            {
                try
                {
                    if (File.Exists(tempFileName))
                        File.Delete(tempFileName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при удалении временного файла: {ex.Message}");
                }
            }
            
        }
    }

    private Task OnBarcodeSubmit()
    {
        var messages = _recognizedBarcodes.Select(x => new BarcodeRequestMessage(x));
        var message = new BarcodeRequestMessageBatch(messages.ToList());
        //TODO: make send via massTransit
        
        return Task.CompletedTask;
    }
}