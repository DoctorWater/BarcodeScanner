
using BarcodeDecodeLib.Models.Dtos.Messages.LocationTicket;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Linq;

namespace MauiAndroid.App.Pages
{
    public partial class LocationTicketsPage : ContentPage
    {
        public ObservableCollection<LocationTicketResponseDto> LocationTickets { get; set; }

        public LocationTicketsPage(ICollection<LocationTicketResponseDto> locationTickets)
        {
            InitializeComponent();

            LocationTickets = new ObservableCollection<LocationTicketResponseDto>(locationTickets.OrderBy(t => t.Id).Select(x => LocationTicketDto.GetFrom(x)));

            BindingContext = this;
        }
    }
}
