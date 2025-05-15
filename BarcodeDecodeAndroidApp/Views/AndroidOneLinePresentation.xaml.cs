using BarcodeDecodeLib.Models.Dtos.Messages.TransportOrder;
using MauiAndroid.App.Data.Services;
using MauiAndroid.App.Pages;

namespace MauiAndroid.App.Views;

public partial class AndroidOneLinePresentation : ContentView
{
	private readonly IHttpMessagingService _messagingService = App.Services.GetRequiredService<IHttpMessagingService>();

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
	{
		var page = new TsuChangePage(TSU);
		Navigation.PushAsync(page);
	}

	void OnEditOrder_Clicked(object sender, EventArgs e)
	{
		var page = new OrderChangePage(TSU.TransportOrder);
		Navigation.PushAsync(page);
	}

	void OnRelaunchOrder_Clicked(object sender, EventArgs e)
	{
		var msg = new TransportOrderRelaunchMessage(TSU.TransportOrder.Barcode);
		_messagingService.SendTransportOrderRelaunchMessage(msg);
	}
}
