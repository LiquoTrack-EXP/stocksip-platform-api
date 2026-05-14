using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using LiquoTrack.StocksipPlatform.API.Authentication.Application.Internal.OutboundServices.Token;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Model.Queries;
using LiquoTrack.StocksipPlatform.API.Authentication.Domain.Services;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.IntegrationTests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    public const string TestUserId  = "test-user-id-001";
    public const string TestToken   = "test-token";
    public const string TestAccount = "acc_001";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var mockTokenService = new Mock<ITokenService>();
            mockTokenService
                .Setup(s => s.ValidateToken(TestToken))
                .ReturnsAsync(TestUserId);  
            services.RemoveAll<ITokenService>();
            services.AddSingleton(mockTokenService.Object);
            
            var fakeUser = BuildFakeUser();

            var mockUserQueryService = new Mock<IUserQueryService>();
            mockUserQueryService
                .Setup(s => s.Handle(It.IsAny<GetUserByIdQuery>()))
                .ReturnsAsync(fakeUser);

            services.RemoveAll<IUserQueryService>();
            services.AddScoped(_ => mockUserQueryService.Object);
            
            var hostedServiceDescriptor = services.FirstOrDefault(d =>
                d.ImplementationType?.Name.Contains("SubscriptionsExpirationJob") ?? false);
            if (hostedServiceDescriptor != null)
                services.Remove(hostedServiceDescriptor);
        });
    }

    private static User BuildFakeUser()
    {
        var email = new Email("test@test.com");
        return new User(
            email:          email,
            username:       "testuser",
            hashedPassword: "HashedPassword123!",
            accountId:      TestAccount,
            userRole:       "Admin"   
        );
    }
}