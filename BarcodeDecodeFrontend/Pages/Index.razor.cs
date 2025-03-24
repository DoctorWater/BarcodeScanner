using System.Text;
using BarcodeDecodeFrontend.Data.Models;
using BarcodeDecodeFrontend.Shared.Modals;
using BarcodeDecodeLib.Entities;
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
    private List<BarcodeModel> _recognizedImageBarcodes = new();
    private List<TransportStorageUnit> _foundTsu = new();
    private List<TransportOrder> _foundOrders = new();
    private bool _hasSearched = false;
    private Dictionary<Guid, string> _imageUrls = new();
    private bool _hoverOn = true;
    

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
        _imageUrls.Clear();
        var imageFiles = fileVerifyResult.Where(p => p.Value == "image").Select(x => x.Key).ToList();
        loadedFiles.Clear();
        foreach (var file in imageFiles)
        {
            try
            {
                var id = Guid.NewGuid();
                await using MemoryStream ms = new MemoryStream();
                await file.OpenReadStream().CopyToAsync(ms);
                var url = $"data:{file.ContentType};base64,{Convert.ToBase64String(ms.ToArray())}";
                _imageUrls.Add(id, url);
                await InvokeAsync(StateHasChanged);
                _recognizedImageBarcodes.Add(new BarcodeModel(id, Decoder.Decode(ms.ToArray()) ?? String.Empty));
            }
            catch (Exception ex)
            {
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
                        _recognizedImageBarcodes.Add(new BarcodeModel(Guid.NewGuid(), result));
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

    private async Task OnBarcodeSubmit()
    {
        var messages = _recognizedImageBarcodes.Select(x => new BarcodeRequestMessage(x.Barcode));
        var message = new BarcodeRequestMessageBatch(messages.ToList());
        var response = await BarcodePublisher.SendBarcodeRequest(message);
        _hasSearched = true;
        await UpdatePresentations(response);
    }

    private async Task UpdatePresentations(BarcodeResponseMessageBatch barcodeResponse)
    {
        _foundTsu = barcodeResponse.Messages.SelectMany(x => x.TransportStorageUnits).ToList();
        _foundOrders = barcodeResponse.Messages.SelectMany(x => x.TransportOrders).ToList();
        await InvokeAsync(StateHasChanged);
    }

    private void OpenImageModal(string imageUrl)
    {
        if (_hoverOn is false)
        {
            var parameters = new ModalParameters();
            parameters.Add(nameof(ModalShowImage.ImageUrl), imageUrl);
            Modal.Show<ModalShowImage>("Image", parameters);
        }
    }
}