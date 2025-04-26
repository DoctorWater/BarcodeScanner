
using BarcodeDecodeLib.Models.Dtos.Messages.LocationTicket;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Linq;

namespace MauiAndroid.App.Pages
{
    public partial class LocationTicketsPage : ContentPage
    {
        public ObservableCollection<LocationTicketViewModel> LocationTickets { get; set; }

        public LocationTicketsPage(ICollection<LocationTicketResponseDto> locationTickets)
        {
            InitializeComponent();

            LocationTickets = new ObservableCollection<LocationTicketViewModel>(locationTickets.OrderBy(t => t.Id).Select(x => LocationTicketViewModel.GetFrom(x)));

            BindingContext = this;
        }
    }
}
