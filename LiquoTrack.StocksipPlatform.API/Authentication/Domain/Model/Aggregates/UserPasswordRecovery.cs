namespace LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Aggregates;

public partial class User
{
    /// <summary>
    ///     The recovery password of the gmail service.
    /// </summary>
    public string? RecoveryCode { get; set; }
    
    /// <summary>
    ///     Date when the recovery password expires.
    /// </summary>
    public DateTime? RecoveryCodeExpiration { get; set; }

    /// <summary>
    ///     Method to set the recovery password.
    /// </summary>
    /// <param name="recoveryCode">
    ///     The 6-digit recovery code.
    /// </param>
    /// <param name="expiration">
    ///     The expiration time of the recovery code.
    /// </param>
    public void SetRecoveryCode(string recoveryCode, TimeSpan expiration)
    {
        RecoveryCode = recoveryCode;
        RecoveryCodeExpiration = DateTime.UtcNow.Add(expiration);
    }

    /// <summary>
    ///     Property to check if the recovery code expiration time is valid.
    /// </summary>
    /// <returns>
    ///     True if the recovery code expiration time is valid, false otherwise.
    /// </returns>
    public bool IsRecoveryCodeExpirationTimeValid => RecoveryCodeExpiration > DateTime.UtcNow;
    

    /// <summary>
    ///     Method to clear the recovery code.
    /// </summary>
    public void ClearRecoveryCode()
    {
        RecoveryCode = null;
        RecoveryCodeExpiration = null;   
    }

    /// <summary>
    ///     Method to update the password.
    /// </summary>
    /// <param name="newPassword">
    ///     The new password.
    /// </param>
    public void UpdatePassword(string newPassword) => Password = newPassword;

}