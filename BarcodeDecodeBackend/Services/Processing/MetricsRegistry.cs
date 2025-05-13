using Prometheus;

namespace BarcodeDecodeBackend.Services.Processing;

public static class MetricsRegistry
{
    // === AuthController ===
    public static readonly Counter AuthLoginAttemptsTotal = Metrics.CreateCounter(
        "myapp_auth_login_attempts_total",
        "Общее число попыток логина"
    );
    public static readonly Counter AuthLoginSuccessTotal = Metrics.CreateCounter(
        "myapp_auth_login_success_total",
        "Число успешных логинов"
    );
    public static readonly Counter AuthLoginFailureTotal = Metrics.CreateCounter(
        "myapp_auth_login_failure_total",
        "Число неудачных попыток логина"
    );
    public static readonly Histogram AuthLoginDuration = Metrics.CreateHistogram(
        "myapp_auth_login_duration_seconds",
        "Duration of Login handler in seconds",
        new HistogramConfiguration { Buckets = Histogram.LinearBuckets(start: 0.01, width: 0.05, count: 20) }
    );

    // === BarcodeController ===
    public static readonly Counter BarcodeBatchRequestsTotal = Metrics.CreateCounter(
        "myapp_barcode_batch_requests_total",
        "Число запросов /batch"
    );
    public static readonly Counter BarcodeBatchItemsProcessedTotal = Metrics.CreateCounter(
        "myapp_barcode_batch_items_processed_total",
        "Количество штрихкодов, обработанных за все запросы"
    );
    public static readonly Counter BarcodeBatchErrorsTotal = Metrics.CreateCounter(
        "myapp_barcode_batch_errors_total",
        "Число ошибок при обработке пакета штрихкодов"
    );
    public static readonly Histogram BarcodeBatchDuration = Metrics.CreateHistogram(
        "myapp_barcode_batch_duration_seconds",
        "Длительность обработки /batch в секундах"
    );

    // === TransportOrderController ===
    public static readonly Counter TransportOrderChangeRequestsTotal = Metrics.CreateCounter(
        "myapp_transportorder_change_requests_total",
        "Число запросов на изменение заказа"
    );
    public static readonly Counter TransportOrderChangeSuccessTotal = Metrics.CreateCounter(
        "myapp_transportorder_change_success_total",
        "Число успешных изменений заказа"
    );
    public static readonly Counter TransportOrderChangeNotFoundTotal = Metrics.CreateCounter(
        "myapp_transportorder_change_notfound_total",
        "Число 404 при изменении заказа"
    );
    public static readonly Histogram TransportOrderChangeDuration = Metrics.CreateHistogram(
        "myapp_transportorder_change_duration_seconds",
        "Длительность обработки изменения заказа"
    );

    public static readonly Counter TransportOrderRelaunchRequestsTotal = Metrics.CreateCounter(
        "myapp_transportorder_relaunch_requests_total",
        "Число запросов на перезапуск заказа"
    );
    public static readonly Counter TransportOrderRelaunchSuccessTotal = Metrics.CreateCounter(
        "myapp_transportorder_relaunch_success_total",
        "Число успешных перезапусков заказа"
    );
    public static readonly Counter TransportOrderRelaunchFailureTotal = Metrics.CreateCounter(
        "myapp_transportorder_relaunch_failure_total",
        "Число неудачных перезапусков заказа"
    );
    public static readonly Histogram TransportOrderRelaunchDuration = Metrics.CreateHistogram(
        "myapp_transportorder_relaunch_duration_seconds",
        "Длительность перезапуска заказа"
    );

    // === TsuController ===
    public static readonly Counter TsuChangeRequestsTotal = Metrics.CreateCounter(
        "myapp_tsu_change_requests_total",
        "Число запросов на изменение TSU"
    );
    public static readonly Counter TsuChangeSuccessTotal = Metrics.CreateCounter(
        "myapp_tsu_change_success_total",
        "Число успешных изменений TSU"
    );
    public static readonly Counter TsuChangeNotFoundTotal = Metrics.CreateCounter(
        "myapp_tsu_change_notfound_total",
        "Число 404 при изменении TSU"
    );
    public static readonly Histogram TsuChangeDuration = Metrics.CreateHistogram(
        "myapp_tsu_change_duration_seconds",
        "Длительность обработки изменения TSU"
    );
}