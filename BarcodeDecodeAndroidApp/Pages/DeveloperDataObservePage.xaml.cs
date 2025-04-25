namespace MauiAndroid.App.Pages
{
    public partial class DeveloperDataObservePage : ContentPage
    {
        public DeveloperDataObservePage(ResponseData responseData)
        {
            InitializeComponent();

            BindingContext = responseData;
        }

        private async void OnTsuTapped(object sender, EventArgs e)
        {
            var frame = sender as Frame;
            var selectedTsu = frame.BindingContext as AndroidTsuDto;
            if (selectedTsu != null)
            {
                selectedTsu.IsExpanded = !selectedTsu.IsExpanded;
            }
        }
    }
}