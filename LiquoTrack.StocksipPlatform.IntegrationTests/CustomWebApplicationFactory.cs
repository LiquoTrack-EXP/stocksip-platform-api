using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Moq;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Seeding;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Domain.Repositories;

namespace LiquoTrack.StocksipPlatform.IntegrationTests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Add Test Authentication Scheme BEFORE any JWT Bearer registration
            // This will override JWT since we add it first
            services.AddAuthentication("Test")
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });

            // Remove SubscriptionsExpirationJob to prevent background task crashes
            var hostedServiceDescriptor = services.FirstOrDefault(d => 
                d.ImplementationType?.Name.Contains("SubscriptionsExpirationJob") ?? false);
            if (hostedServiceDescriptor != null) 
                services.Remove(hostedServiceDescriptor);
        });
    }
}
