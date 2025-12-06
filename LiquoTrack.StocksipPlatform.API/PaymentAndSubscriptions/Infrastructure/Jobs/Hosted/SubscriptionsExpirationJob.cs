using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Application.Internal.OutBoundServices.Jobs.Hosted;
using Microsoft.Extensions.Hosting;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Infrastructure.Jobs.Hosted;

/// <summary>
///     Hosted service that periodically checks for expired pending subscriptions.
/// </summary>
public class SubscriptionsExpirationJob : IHostedService, IDisposable
{
    private readonly ISubscriptionsExpirationService _expirationService;
    private Timer? _timer;

    public SubscriptionsExpirationJob(ISubscriptionsExpirationService expirationService)
    {
        _expirationService = expirationService ?? throw new ArgumentNullException(nameof(expirationService));
    }

    /// <summary>
    ///     Method to start the job.
    /// </summary>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <returns>
    ///     A task representing the asynchronous operation.
    /// </returns>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(async _ => await _expirationService.CheckPendingSubscriptionsAsync(),
            null, TimeSpan.Zero, TimeSpan.FromMinutes(1));

        return Task.CompletedTask;
    }

    /// <summary>
    ///     Method to stop the job.
    /// </summary>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <returns>
    ///     A task representing the asynchronous operation.
    /// </returns>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Method to dispose the job.
    /// </summary>
    public void Dispose()
    {
        _timer?.Dispose();
    }
}