using LiquoTrack.StocksipPlatform.API.ProfileManagement.Domain.Model.Commands;

namespace LiquoTrack.StocksipPlatform.API.ProfileManagement.Interfaces.REST.Assemblers;

/// <summary>
///     Assembler to convert REST resource data into a DeleteProfileByIdCommand.
/// </summary>
public class DeleteProfileCommandFromResourceAssembler
{
    /// <summary>
    ///     Static method to convert REST resource data into a DeleteProfileByIdCommand.
    /// </summary>
    /// <param name="profileId">
    ///     The ID of the profile to delete.
    /// </param>
    /// <returns>
    ///     A DeleteProfileByIdCommand representing the profile to delete.
    /// </returns>
    public static DeleteProfileByIdCommand ToCommandFromResource(string profileId)
    {
        return new DeleteProfileByIdCommand(profileId);
    }
}