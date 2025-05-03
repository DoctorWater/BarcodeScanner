using System.Collections.ObjectModel;

namespace MauiAndroid.App.Pages;

public partial class ClientDataObservePage : ContentPage
{
	public ObservableCollection<TransportStorageUnitViewModel> TSUs { get; }

	public ClientDataObservePage(IEnumerable<TransportStorageUnitViewModel> tsus)
	{
		InitializeComponent();

		TSUs = new ObservableCollection<TransportStorageUnitViewModel>(tsus);
		BindingContext = this;
	}
}