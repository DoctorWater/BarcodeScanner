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
    private List<string> _recognizedImageBarcodes = new();

    private async Task LoadFiles(InputFileChangeEventArgs e)
    {
        var files = e.GetMultipleFiles();
        var fileVerifyResult = VerifyFiles(files);
        var userMessage = CreateUserMessage(fileVerifyResult);
        if (userMessage != string.Empty)
        {
            var parameters = new ModalParameters();
            parameters.Add(nameof(ModalAlert.Message), userMessage);

            Modal.Show<ModalAlert>("ALERT", parameters);
        }

        await LoadPhotoFiles(fileVerifyResult);
        await LoadVideoFiles(fileVerifyResult);
    }


    private async Task LoadPhotoFiles(Dictionary<IBrowserFile, string?> fileVerifyResult)
    {
        var imageFiles = fileVerifyResult.Where(p => p.Value == "image").Select(x => x.Key).ToList();
        loadedFiles.Clear();
        foreach (var file in imageFiles)
        {
            try
            {
                await using MemoryStream ms = new MemoryStream();
                await file.OpenReadStream().CopyToAsync(ms);
                _recognizedImageBarcodes.Add(Decoder.Decode(ms.ToArray()) ?? String.Empty);
            }
            catch (Exception ex)
            {
            }
        }
    }

    private async Task RemoveBarcode(string barcode)
    {
        _recognizedImageBarcodes.Remove(barcode);
        await InvokeAsync(StateHasChanged);
    }
    
    private async Task LoadVideoFiles(Dictionary<IBrowserFile, string?> fileVerifyResult)
    {
        var videoFiles = fileVerifyResult.Where(p => p.Value == "video").Select(x => x.Key).ToList();
        loadedFiles.Clear();

        foreach (var file in videoFiles)
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

                var frames = VideoProcessor.GetVideoFrames(tempFileName, extension);
                foreach (var frame in frames)
                {
                    var result = Decoder.Decode(frame);
                    if (result is not null)
                    {
                        _recognizedImageBarcodes.Add(result);
                        break;
                    }
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

    private Dictionary<IBrowserFile, string?> VerifyFiles(IReadOnlyList<IBrowserFile> files)
    {
        Dictionary<IBrowserFile, string?> fileVerifyResult = new Dictionary<IBrowserFile, string?>();
        foreach (var file in files)
        {
            if (file.ContentType.Contains("video"))
            {
                fileVerifyResult.Add(file, "video");
            }
            else if (file.ContentType.Contains("image"))
            {
                fileVerifyResult.Add(file, "image");
            }
            else
            {
                fileVerifyResult.Add(file, null);
            }
        }

        return fileVerifyResult;
    }

    private string CreateUserMessage(Dictionary<IBrowserFile, string?> fileVerifyResult)
    {
        var sb = new StringBuilder();
        foreach (var pair in fileVerifyResult)
        {
            if (pair.Value is null)
            {
                sb.AppendLine($"File {pair.Key.Name} is invalid");
            }
        }

        return sb.ToString();
    }

    private Task OnBarcodeSubmit()
    {
        var messages = _recognizedImageBarcodes.Select(x => new BarcodeRequestMessage(x));
        var message = new BarcodeRequestMessageBatch(messages.ToList());
        //TODO: make send via massTransit

        return Task.CompletedTask;
    }
}