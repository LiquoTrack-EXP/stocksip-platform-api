using System.Net.Mime;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Services;
using LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.External.Google.Requests;
using LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.Pipeline.Middleware.Attributes;
using LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.REST.Resources;
using LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Interfaces.REST.Controllers
{
    using LiquoTrack.StocksipPlatform.API.Authentication.Application.Internal.OutboundServices.Authentication;
    using LiquoTrack.StocksipPlatform.API.Authentication.Application.Internal.OutboundServices.Token;
    using Microsoft.Extensions.Logging;

    [ApiController]
    [Route("api/v1")]
    [Produces(MediaTypeNames.Application.Json)]
    [SwaggerTag("Authentication endpoints, including Google Sign-In.")]
    public class AuthenticationController(
        IExternalAuthService externalAuthService,
        IUserCommandService userCommandService,
        ITokenService tokenService,
        ILogger<AuthenticationController> logger) : ControllerBase
    {
        /// <summary>
        /// Sign in with Google Identity Services ID token. Returns app JWT and user info.
        /// </summary>
        [HttpPost("authentication/google")] 
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Sign in with Google",
            Description = "Accepts a Google ID token from GIS and returns an app JWT with user info",
            OperationId = "GoogleSignIn")]
        [SwaggerResponse(StatusCodes.Status200OK, "Authenticated", typeof(AuthenticatedUserResource))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid ID token")]
        public async Task<IActionResult> SignInWithGoogle([FromBody] GoogleAuthRequest request)
        {
            if (request is null || string.IsNullOrWhiteSpace(request.IdToken))
                return BadRequest(new { Error = "IdToken is required" });

            var validation = await externalAuthService.ValidateIdTokenAsync(request.IdToken);
            if (!validation.Success || string.IsNullOrWhiteSpace(validation.Email) || string.IsNullOrWhiteSpace(validation.ProviderUserId))
            {
                logger.LogWarning("Google auth failed: {Error}", validation.Error);
                return BadRequest(new { Error = validation.Error ?? "Invalid Google token" });
            }

            var user = await userCommandService.CreateOrUpdateFromExternalAsync(
                validation.ProviderUserId!, validation.Email!, validation.Name);

            var jwt = tokenService.GenerateToken(user);

            var resource = new AuthenticatedUserResource(
                jwt,
                user.Id.ToString(),
                user.Email.ToString(),
                user.Username,
                user.AccountId?.GetId ?? string.Empty
            );

            return Ok(resource);
        }
        #region Authentication

        /// <summary>
        /// Signs in a user with email and password
        /// </summary>
        /// <param name="signInResource">User credentials</param>
        [HttpPost("sign-in")]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Sign in",
            Description = "Sign in with email and password"
        )]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SignIn([FromBody] SignInResource signInResource)
        {
            logger.LogInformation("Sign in attempt for user: {Email}", signInResource.Email);

            try
            {
                var signInCommand = SignInCommandFromResourceAssembler.ToCommandFromResource(signInResource);
                var (user, token) = await userCommandService.Handle(signInCommand);

                logger.LogInformation("Successful sign in for user: {Email}", user.Email);
                var resource = AuthenticatedUserResourceFromEntityAssembler.ToResourceFromEntity(user, token);
                return Ok(resource);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Sign in failed for user: {Email}", signInResource?.Email);
                return Unauthorized(new
                {
                    error = "Authentication failed",
                    message = "Invalid email or password",
                    requestId = HttpContext.TraceIdentifier
                });
            }
        }

        /// <summary>
        /// Registers a new user
        /// </summary>
        /// <param name="signUpResource">User registration details</param>
        [AllowAnonymous]
        [HttpPost("sign-up")]
        [SwaggerOperation(
            Summary = "Sign up",
            Description = "Register a new user"
        )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SignUp([FromBody] SignUpResource signUpResource)
        {
            logger.LogInformation("New user registration: {Email}", signUpResource.Email);

            try
            {
                var signUpCommand = SignUpCommandFromResourceAssembler.ToCommandFromResource(signUpResource);
                var result = await userCommandService.Handle(signUpCommand);

                logger.LogInformation("User registered successfully: {Email}", signUpResource.Email);
                return Ok(new { message = "User registered successfully" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Registration failed for user: {Email}", signUpResource.Email);
                return HandleException(ex);
            }
        }

        #endregion

        private IActionResult HandleException(Exception ex)
        {
            var errorId = Guid.NewGuid().ToString();
            logger.LogError(ex, "Error ID: {ErrorId}", errorId);

            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                error = "An unexpected error occurred",
                errorId,
                details = ex.Message
            });
        }
    }
}