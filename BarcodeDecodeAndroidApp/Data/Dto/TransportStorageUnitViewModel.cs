using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BarcodeDecodeLib.Models.Dtos.Messages.LocationTicket;
using BarcodeDecodeLib.Models.Dtos.Messages.Tsu;
using BarcodeDecodeLib.Models.Enums;

public class TransportStorageUnitViewModel : INotifyPropertyChanged
{
    const double TICKET_HEADER_HEIGHT = 50;
    const double TICKET_ROW_HEIGHT = 40;
    private bool _isExpanded;

    public int Id { get; init; }
    public string? Barcode { get; init; }
    public TsuStatusEnum Status { get; init; }
    public DateTimeOffset? UpdatedOn { get; init; }
    public bool IsOkay { get; init; }
    public ObservableCollection<LocationTicketViewModel> LocationTicketDtos { get; set; } = new();
    public TransportOrderViewModel? TransportOrder { get; init; }
    public double DataGridHeight { get; }


    public TransportStorageUnitViewModel()
    {
    }

    public TransportStorageUnitViewModel(TsuResponseMessage transportStorageUnit)
    {
        Id = transportStorageUnit.Id;
        Barcode = transportStorageUnit.Barcode;
        Status = transportStorageUnit.Status;
        UpdatedOn = transportStorageUnit.UpdatedOn;
        TransportOrder = transportStorageUnit.TransportOrder is not null ? new TransportOrderViewModel(transportStorageUnit.TransportOrder) : null;

        IsExpanded = false;
        IsOkay = DefineIsOkay(transportStorageUnit);
        LocationTicketDtos = new ObservableCollection<LocationTicketViewModel>(transportStorageUnit.LocationTickets
            .Select(GetFromLocationTicketDto)
            .OrderBy(x => x.ID)
            .ToList());

        var count = transportStorageUnit.LocationTickets.Count;
        DataGridHeight = TICKET_HEADER_HEIGHT + count * TICKET_ROW_HEIGHT;
    }

    private bool DefineIsOkay(TsuResponseMessage tsu)
    {
        var isAnyTransportationWrong = tsu.LocationTickets.Any(x =>
            x.Status != TransportLocationTicketStatus.OnTrack &&
            x.ArrivedAtLocation is not null &&
            !x.PlannedLocations
                .Contains(x.ArrivedAtLocation.Value));

        return tsu.Status != TsuStatusEnum.None && !isAnyTransportationWrong;
    }

    private LocationTicketViewModel GetFromLocationTicketDto(LocationTicketResponseDto ticket)
    {
        var plannedLocation = ticket.PlannedLocations.Any() ? ticket.PlannedLocations[0] : 0;
        return new LocationTicketViewModel
        {
            ID = ticket.Id,
            DepartureLocation = ticket.DepartureLocation,
            Status = ticket.Status.ToString(),
            PlannedLocation = plannedLocation,
            ArrivedAtLocation = ticket.ArrivedAtLocation,
            ArrivedOn = ticket.ArrivedOn,
            IsSuccess = plannedLocation.Equals(ticket.ArrivedAtLocation)
        };
    }

    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            if (_isExpanded != value)
            {
                _isExpanded = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}