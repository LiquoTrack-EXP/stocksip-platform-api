using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Services;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Application.Internal.CommandServices;

/// <summary>
///     Implementation of the <see cref="IAccountCommandService"/> interface for handling account-related commands.
/// </summary>
/// <param name="accountRepository">
///     The repository used to manage Account entities.
/// </param>
public class AccountCommandService(
    IAccountRepository accountRepository,
    IPlanRepository planRepository) : IAccountCommandService
{
    /// <summary>
    ///     Handles the creation of a new account.
    /// </summary>
    /// <param name="command">
    ///     The command containing the details for creating a new account.
    /// </param>
    /// <returns>
    ///     A newly created <see cref="Account"/> instance, or null if the creation failed.
    /// </returns>
    public async Task<Account?> Handle(CreateAccountCommand command)
    {
        var account = new Account(command);
        await accountRepository.AddAsync(account);
        return account;
    }

    /// <summary>
    ///     Handles adding an address to an existing account.
    /// </summary>
    /// <param name="command">
    ///     The command containing the details for adding an address to an account.
    /// </param>
    /// <returns>
    ///     A task representing the asynchronous operation.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the account is not found.
    /// </exception>
    public async Task Handle(AddAddressToAccountCommand command)
    {
        var account = await accountRepository.FindByIdAsync(command.AccountId);
    
        if (account == null)
            throw new InvalidOperationException($"Account with ID {command.AccountId} not found.");

        var address = new Address(
            command.Street,
            command.City,
            command.State,
            command.Country,
            command.ZipCode
        );

        account.AddAddress(address);
    
        Console.WriteLine($"Updating account {command.AccountId} with {account.Addresses.Count} addresses");
    
        await accountRepository.UpdateAsync(account);
    
        Console.WriteLine("Update completed");
    }
}