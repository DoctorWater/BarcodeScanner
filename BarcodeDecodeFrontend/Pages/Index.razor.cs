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

        var imageFiles = fileVerifyResult.Where(p => p.Value is true).Select(x => x.Key).ToList();
        await UploadImageFiles(imageFiles);
    }

    private Dictionary<IBrowserFile, bool> VerifyFiles(IReadOnlyList<IBrowserFile> files)
    {
        Dictionary<IBrowserFile, bool> fileVerifyResult = new Dictionary<IBrowserFile, bool>();
        foreach (var file in files)
        {
            fileVerifyResult.Add(file, file.ContentType.Contains("image"));
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
                sb.AppendLine($"File {pair.Key.Name} is not an image");
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
                _recognizedBarcodes.Add(Decoder.DecodeFromSerializedImage(ms.ToArray()) ?? String.Empty);
            }
            catch (Exception ex)
            {
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