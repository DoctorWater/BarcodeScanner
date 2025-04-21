using BarcodeDecodeFrontend.Data.Dtos;
using BarcodeDecodeFrontend.Shared.Modals;
using BarcodeDecodeLib.Models.Dtos.Messages.TransportOrder;
using BarcodeDecodeLib.Models.Dtos.Messages.Tsu;
using BarcodeDecodeLib.Models.Enums;
using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;

namespace BarcodeDecodeFrontend.Shared;

public partial class OneLinePresentation : ComponentBase
{
    [CascadingParameter] IModalService Modal { get; set; } = default!;

    [Parameter] public TsuResponseDto Tsu { get; set; }
    [Parameter] public TransportOrderResponseDto TransportOrder { get; set; }

    private bool IsExtended { get; set; } = false;

    private string GetTsuStatusColor()
    {
        if (Tsu is null)
            return "text-danger";

        return Tsu.Status switch
        {
            TsuStatusEnum.Active => "text-successful",
            TsuStatusEnum.Closed => "text-warning",
            _ => "text-danger"
        };
    }

    private string GetOrderStatusColor()
    {
        if (TransportOrder is null)
            return "text-danger";
        return TransportOrder.Status switch
        {
            TransportOrderStatusEnum.Created => "text-successful",
            TransportOrderStatusEnum.Active => "text-successful",
            TransportOrderStatusEnum.Closed => "text-warning",
            TransportOrderStatusEnum.Cancelled => "text-warning",
            TransportOrderStatusEnum.Error => "text-danger",
            _ => "text-danger",
        };
    }

    private void GetAdditionalInfo()
    {
        IsExtended = !IsExtended;
    }

    private string GetBorder()
    {
        return IsExtended ? "border-bottom" : "";
    }

    private async Task EditTsu()
    {
        var parameters = new ModalParameters();
        parameters.Add(nameof(ModalEditTsu.Item), TsuEditDto.GetFrom(Tsu));
        var modal = Modal.Show<ModalEditTsu>(Tsu?.Barcode ?? "BARCODE NOT FOUND", parameters);
        var result = (TsuEditDto?)(await modal.Result).Data;
        if (result is not null)
        {
            var message = new TsuChangeMessage(result.Id)
            {
                Barcode = result.Barcode,
                Status = result.Status
            };
            var updatedTsu = await BarcodeMessagePublisher.SendTsuChangeMessage(message);
            if (updatedTsu is not null)
                Tsu = updatedTsu;
            await InvokeAsync(StateHasChanged);
        }
    }

    private async Task EditOrder()
    {
        var parameters = new ModalParameters();
        parameters.Add(nameof(ModalEditOrder.Item), OrderEditDto.GetFrom(TransportOrder));
        var modal = Modal.Show<ModalEditOrder>(TransportOrder.Barcode, parameters);

        var result = (OrderEditDto?)(await modal.Result).Data;
        if (result is not null)
        {
            var message = new TransportOrderChangeMessage(result.Id)
            {
                Status = result.Status,
                Barcode = result.Barcode
            };
            var updatedOrder = await BarcodeMessagePublisher.SendTransportOrderChangeMessage(message);
            if(updatedOrder is not null)
                TransportOrder = updatedOrder;
            await InvokeAsync(StateHasChanged);
        }
    }
}