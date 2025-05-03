using MauiAndroid.App.Pages;

namespace MauiAndroid.App.Views;

public partial class AndroidOneLinePresentation : ContentView
{
	public static readonly BindableProperty TSUProperty =
			BindableProperty.Create(
				nameof(TSU),
				typeof(TransportStorageUnitViewModel),
				typeof(AndroidOneLinePresentation),
				propertyChanged: OnTsuChanged);

	public TransportStorageUnitViewModel TSU
	{
		get => (TransportStorageUnitViewModel)GetValue(TSUProperty);
		set => SetValue(TSUProperty, value);
	}

	public event EventHandler? EditTsuRequested;
	public event EventHandler? EditOrderRequested;
	public event EventHandler? RelaunchOrderRequested;

	public AndroidOneLinePresentation()
	{
		InitializeComponent();
	}

	private static void OnTsuChanged(BindableObject bindable, object oldVal, object newVal)
	{
		if (bindable is AndroidOneLinePresentation view
			&& newVal is TransportStorageUnitViewModel vm)
		{
			view.BindingContext = vm;
		}
	}

	void OnEditTsu_Clicked(object sender, EventArgs e)
		=> EditTsuRequested?.Invoke(this, EventArgs.Empty);

	void OnEditOrder_Clicked(object sender, EventArgs e)
		=> EditOrderRequested?.Invoke(this, EventArgs.Empty);

	void OnRelaunchOrder_Clicked(object sender, EventArgs e)
		=> RelaunchOrderRequested?.Invoke(this, EventArgs.Empty);

	async void OnDetailsClicked(object sender, EventArgs e)
	{
		var page = new LocationTicketsPage{ BindingContext = TSU.LocationTicketDtos };
		await Navigation.PushAsync(page);
	}
}
