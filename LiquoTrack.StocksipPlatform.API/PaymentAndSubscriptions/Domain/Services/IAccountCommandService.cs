using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Commands;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Services;

/// <summary>
///     Service interface for handling account-related commands.
/// </summary>
public interface IAccountCommandService
{
    /// <summary>
    ///     Method to handle the creation of a new account.
    /// </summary>
    /// <param name="command">
    ///     The command containing the details for creating a new account.
    /// </param>
    /// <returns>
    ///     The newly created account.
    ///     Or null if the account could not be created.
    /// </returns>
    Task<Account?> Handle(CreateAccountCommand command);

    /// <summary>
    ///     Method to handle adding an address to an existing account.
    /// </summary>
    /// <param name="command">
    ///     The command containing the details for adding an address to an account.
    /// </param>
    /// <returns>
    ///     A task representing the asynchronous operation.
    /// </returns>
    Task Handle(AddAddressToAccountCommand command);
}