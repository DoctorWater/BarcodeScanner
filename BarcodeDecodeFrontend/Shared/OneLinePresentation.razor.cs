using BarcodeDecodeFrontend.Data.Dtos;
using BarcodeDecodeFrontend.Shared.Modals;
using BarcodeDecodeLib.Entities;
using BarcodeDecodeLib.Models.Enums;
using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;

namespace BarcodeDecodeFrontend.Shared;

public partial class OneLinePresentation : ComponentBase
{
    [CascadingParameter] IModalService Modal { get; set; } = default!;

    [Parameter] public TransportStorageUnit? Tsu { get; set; }
    [Parameter] public TransportOrder? Order { get; set; }

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
        if (Order is null)
            return "text-danger";
        return Order.Status switch
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
        parameters.Add(nameof(ModalEditTsu.Item), TsuDto.GetFrom(Tsu));
        var modal = Modal.Show<ModalEditTsu>(Tsu?.Barcode ?? "BARCODE NOT FOUND", parameters);
        var result = (TsuDto?)(await modal.Result).Data;
        if (result is not null)
        {
            UpdateTsu(Tsu, result);
            await BarcodeMessagePublisher.SendTsuChangeMessage(Tsu);
            await InvokeAsync(StateHasChanged);
        }
    }

    private void UpdateTsu(TransportStorageUnit tsu, TsuDto data)
    {
        tsu.Barcode = data.Barcode ?? tsu.Barcode;
        tsu.UpdatedOn = data.UpdatedOn ?? tsu.UpdatedOn;
        tsu.Status = data.Status;
    }

    private async Task EditOrder()
    {
        var parameters = new ModalParameters();
        parameters.Add(nameof(ModalEditOrder.Item), OrderDto.GetFrom(Order));
        var modal = Modal.Show<ModalEditOrder>(Order.Barcode, parameters);

        var result = (OrderEditDto?)(await modal.Result).Data;
        if (result is not null)
        {
            UpdateOrder(Order, result);
            await InvokeAsync(StateHasChanged);
        }
    }

    private void UpdateOrder(TransportOrder order, OrderEditDto resultData)
    {
        order.Barcode = resultData.Barcode ?? order.Barcode;
        if (Enum.TryParse<TransportOrderStatusEnum>(resultData.Status, out var statusEnum))
            order.Status = statusEnum;
    }
}