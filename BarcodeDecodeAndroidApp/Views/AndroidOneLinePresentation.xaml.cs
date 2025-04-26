using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MauiAndroid.App.Views;

public partial class AndroidOneLinePresentation : ContentView
{
	public AndroidOneLinePresentation()
	{
		InitializeComponent();
		BindingContext = this;
	}

	public static readonly BindableProperty ClientPresentationProperty = BindableProperty.Create(
		nameof(ClientPresentation),
		typeof(ClientPresentationDto),
		typeof(AndroidOneLinePresentation),
		default(ClientPresentationDto),
		propertyChanged: OnClientPresentationChanged);

	public ClientPresentationDto ClientPresentation
	{
		get => (ClientPresentationDto)GetValue(ClientPresentationProperty);
		set => SetValue(ClientPresentationProperty, value);
	}

	private static void OnClientPresentationChanged(BindableObject bindable, object oldValue, object newValue)
	{
		var control = (AndroidOneLinePresentation)bindable;
		control.OnClientPresentationChanged();
	}

	private void OnClientPresentationChanged()
	{
		OnPropertyChanged(nameof(Order));
		OnPropertyChanged(nameof(LocationTickets));
	}

	public TransportOrderViewModel Order => ClientPresentation?.Order;
	public IEnumerable<LocationTicketViewModel> LocationTickets => ClientPresentation?.LocationTickets;

	public new event PropertyChangedEventHandler PropertyChanged;
	protected new void OnPropertyChanged([CallerMemberName] string propertyName = null)
	{
		var handler = PropertyChanged;
		handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}