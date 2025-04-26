using BarcodeDecodeLib.Models.Dtos.Messages.Barcode;

namespace MauiAndroid.App.Pages
{
    public partial class DeveloperDataObservePage : ContentPage
    {
        public DeveloperDataObservePage(BackendResponseViewModel responseData)
        {
            InitializeComponent();

            BindingContext = responseData;
        }

        private async void OnTsuTapped(object sender, EventArgs e)
        {
            var frame = sender as Frame;
            var selectedTsu = frame.BindingContext as TransportStorageUnitViewModel;
            if (selectedTsu != null)
            {
                selectedTsu.IsExpanded = !selectedTsu.IsExpanded;
            }
        }
    }
}