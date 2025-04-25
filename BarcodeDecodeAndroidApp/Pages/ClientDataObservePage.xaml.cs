using System.Collections.ObjectModel;

namespace MauiAndroid.App.Pages;

public partial class ClientDataObservePage : ContentPage
{
	public ObservableCollection<ClientPresentationDto> ClientPresentations { get; set; }

	public ClientDataObservePage(IEnumerable<ClientPresentationDto> dtos)
	{
		InitializeComponent();

		ClientPresentations = new ObservableCollection<ClientPresentationDto>(dtos){};

		BindingContext = this;
	}
}