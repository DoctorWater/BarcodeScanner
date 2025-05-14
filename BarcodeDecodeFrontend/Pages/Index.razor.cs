using System.Text;
using BarcodeDecodeFrontend.Data.Models;
using BarcodeDecodeFrontend.Shared.Modals;
using BarcodeDecodeLib.Models.Dtos.Messages.Barcode;
using BarcodeDecodeLib.Models.Dtos.Messages.TransportOrder;
using BarcodeDecodeLib.Models.Dtos.Messages.Tsu;
using BarcodeDecodeLib.Models.Dtos.Models;
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
    private List<BarcodeModel> _recognizedImageBarcodes = new();
    private List<TsuResponseMessage> _foundTsu = new();
    private List<TransportOrderResponseMessage> _foundOrders = new();
    private bool _hasSearched = false;
    private Dictionary<Guid, string> _imageUrls = new();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;
        var token = await LocalStorage.GetItemAsync<string>("authToken");
        TokenProvider.Token = token;
    }

    private async Task LoadFiles(InputFileChangeEventArgs e)
    {
        _imageUrls.Clear();
        loadedFiles.Clear();
        _recognizedImageBarcodes.Clear();
        var files = e.GetMultipleFiles();
        Logger.LogDebug("{fileCount} files uploaded", files.Count);
        var fileVerifyResult = VerifyFiles(files);
        var userMessage = CreateUserMessage(fileVerifyResult);
        if (userMessage != string.Empty)
        {
            ToastService.ShowWarning(userMessage);
        }

        await LoadPhotoFiles(fileVerifyResult);
        await LoadVideoFiles(fileVerifyResult);
        
        await InvokeAsync(StateHasChanged);
    }


    private async Task LoadPhotoFiles(Dictionary<IBrowserFile, string?> fileVerifyResult)
    {
        var imageFiles = fileVerifyResult
            .Where(p => p.Value == "image")
            .Select(x => x.Key)
            .ToList();

        var processingTasks = imageFiles.Select(ProcessFileAsync);

        await Task.WhenAll(processingTasks);

        Logger.LogDebug("{fileCount} photo files processed", imageFiles.Count);

        async Task ProcessFileAsync(IBrowserFile file)
        {
            var id = Guid.NewGuid();
            await using var ms = new MemoryStream();

            await file.OpenReadStream().CopyToAsync(ms);

            var base64 = Convert.ToBase64String(ms.ToArray());
            var url = $"data:{file.ContentType};base64,{base64}";

            lock (_imageUrls)
            {
                _imageUrls.Add(id, url);
            }

            var decoded = Decoder.Decode(ms.ToArray()) ?? string.Empty;
            lock (_recognizedImageBarcodes)
            {
                _recognizedImageBarcodes.Add(new BarcodeModel(id, decoded));
            }
        }
    }


    private async Task RemoveBarcode(BarcodeModel barcodeModel)
    {
        _recognizedImageBarcodes.Remove(barcodeModel);
        _imageUrls.Remove(barcodeModel.Id);
        await InvokeAsync(StateHasChanged);
    }

    private async Task LoadVideoFiles(Dictionary<IBrowserFile, string?> fileVerifyResult)
    {
        var videoFiles = fileVerifyResult.Where(p => p.Value == "video").Select(x => x.Key).ToList();
        loadedFiles.Clear();

        var processingTasks = videoFiles.Select(ProcessFileAsync);

        await Task.WhenAll(processingTasks);

        async Task ProcessFileAsync(IBrowserFile file)
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
                        _recognizedImageBarcodes.Add(new BarcodeModel(Guid.NewGuid(), result));
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning("{filename} upload failed: {ex.Message}", file.Name, ex.Message);
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
                    Logger.LogWarning("Error deleting temp file: {ex.Message}", ex.Message);
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
                sb.AppendLine($"Файл {pair.Key.Name} имеет некорректный формат.");
            }
        }

        return sb.ToString();
    }

    private async Task OnBarcodeSubmit()
    {
        var messages = _recognizedImageBarcodes.Select(x => new BarcodeRequestMessage(x.Barcode));
        var message = new BarcodeRequestMessageBatch(messages.ToList());
        try
        {
            var response = await HttpPublisher.SendBarcodeRequest(message);
            _hasSearched = true;
            await UpdatePresentations(response);
        }
        catch (UnauthorizedAccessException)
        {
            Logger.LogInformation("User tried to send messages without authorization: {messages}", messages);
            ToastService.ShowError("Пользователь не авторизован");
        }
    }

    private async Task UpdatePresentations(BarcodeResponseMessageBatch barcodeResponse)
    {
        _foundTsu = barcodeResponse.Messages.SelectMany(x => x.TransportStorageUnits).ToList();
        _foundOrders = barcodeResponse.Messages.SelectMany(x => x.TransportOrders).ToList();
        await InvokeAsync(StateHasChanged);
    }

    private void OpenImageModal(string imageUrl)
    {
        var parameters = new ModalParameters();
        parameters.Add(nameof(ModalShowImage.ImageUrl), imageUrl);
        Modal.Show<ModalShowImage>("Image", parameters);
    }
}