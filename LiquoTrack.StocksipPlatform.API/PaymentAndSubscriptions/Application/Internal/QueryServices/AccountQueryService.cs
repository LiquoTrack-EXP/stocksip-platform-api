using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Queries;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Services;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Application.Internal.QueryServices;

/// <summary>
///     Implementation of the <see cref="IAccountQueryService"/> interface.
/// </summary>
/// <param name="accountRepository">
///     The repository for handling account-related operations.
/// </param>
public class AccountQueryService(IAccountRepository accountRepository) : IAccountQueryService
{
    /// <summary>
    ///     Method to handle the retrieval of an account by its ID.   
    /// </summary>
    /// <param name="query">
    ///     The query object containing the account ID. 
    /// </param>
    /// <returns>
    ///     The account with the specified ID.
    /// </returns>
    public async Task<Account?> Handle(GetAccountByIdQuery query)
    {
        return await accountRepository.FindByIdAsync(query.AccountId);
    }

    public async Task<string?> Handle(GetAccountStatusByIdQuery query)
    {
        return await accountRepository.GetAccountStatusByIdAsync(query.AccountId);
    }
    
    public async Task<IEnumerable<Account>> Handle(GetAccountsByRoleQuery query)
    {
        return await accountRepository.FindAccountsByRoleAsync(query.Role);
    }
}