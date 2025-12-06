using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.Authentication.Application.Internal.OutboundServices.Token;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Services;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Model.Queries;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.Tokens.JWT.Services
{
    /// <summary>
    /// This class is used to generate and validate JWT tokens.
    /// </summary>
    public class TokenService : ITokenService
    {
        private readonly string _secret;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly IAccountQueryService _accountQueryService;

        public TokenService(IConfiguration configuration, IAccountQueryService accountQueryService)
        {
            var jwtSection = configuration.GetSection("Jwt");
            _secret = jwtSection["Secret"] ?? throw new ArgumentNullException("JWT Secret is not configured");
            _issuer = jwtSection["Issuer"] ?? "LiquoTrack.API";
            _audience = jwtSection["Audience"] ?? "LiquoTrack.Clients";
            _accountQueryService = accountQueryService;

            if (string.IsNullOrEmpty(_secret))
            {
                throw new ArgumentException("JWT Secret is not configured");
            }
        }

        /// <summary>
        /// Generate a JWT token for the given user.
        /// </summary>
        /// <param name="user">The user for whom the token is generated.</param>
        /// <returns>The generated JWT token as a string.</returns>
        public string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secret);
            var roleValue = "User";

            try
            {
                var accountId = user.AccountId?.GetId;
                if (!string.IsNullOrWhiteSpace(accountId))
                {
                    var account = _accountQueryService
                        .Handle(new GetAccountByIdQuery(accountId))
                        .GetAwaiter().GetResult();
                    if (account != null)
                    {
                        roleValue = account.Role.ToString();
                    }
                }
            }
            catch
            {
                roleValue = "User";
            }
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Sid, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username ?? string.Empty),
                    new Claim(ClaimTypes.Email, user.Email?.ToString() ?? string.Empty),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Role, roleValue)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), 
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Verify the validity of a JWT token and return the user ID if valid.
        /// </summary>
        /// <param name="token">The token to validate.</param>
        /// <returns>The user ID if the token is valid; otherwise, null.</returns>
        public async Task<string?> ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secret);
            
            try
            {
                var tokenValidationResult = await tokenHandler.ValidateTokenAsync(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    ClockSkew = TimeSpan.Zero
                });

                var jwtToken = (JwtSecurityToken)tokenValidationResult.SecurityToken;
                var userId = jwtToken.Claims.First(claim => claim.Type == ClaimTypes.Sid).Value;
                return userId;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}