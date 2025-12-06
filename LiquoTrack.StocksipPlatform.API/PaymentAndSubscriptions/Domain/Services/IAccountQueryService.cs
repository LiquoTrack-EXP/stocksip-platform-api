using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Queries;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Services;

/// <summary>
///     Query service interface for retrieving account and business information.
/// </summary>
public interface IAccountQueryService
{
    /// <summary>
    ///     Method to handle the retrieval of an account by its ID.
    /// </summary>
    /// <param name="query">
    ///     The query object containing the account ID.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation.
    ///     The task result contains the account if found; otherwise, null.
    /// </returns>
    Task<Account?> Handle(GetAccountByIdQuery query);

    /// <summary>
    ///     Method to handle the retrieval of an account's status by its ID.'
    /// </summary>
    /// <param name="query">
    ///     The query object containing the account ID.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation.
    /// </returns>
    Task<string?> Handle(GetAccountStatusByIdQuery query);

    /// <summary>
    /// Method to handle the retrieval of accounts by their role.
    /// </summary>
    /// <param name="query">
    /// The query object containing the role information to filter accounts.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a collection of accounts matching the specified role.
    /// </returns>
    Task<IEnumerable<Account>> Handle(GetAccountsByRoleQuery query);
}