using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using FastPaceTransferTest2022.Api.Models.Responses;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace FastPaceTransferTest2022.Api.Tests
{
    public class WalletServiceShould : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly HttpClient httpClient;
        private readonly Faker _faker;
        private readonly UserResponse _user;

        public WalletServiceShould(WebApplicationFactory<Startup> factory)
        {
            httpClient = factory.CreateClient();
            _faker = new Faker();
            
            _user = new UserResponse
            {
                Id = Guid.NewGuid().ToString("N"),
                EmailAddress = _faker.Person.Email,
                FirstName = _faker.Person.FirstName,
                LastName = _faker.Person.LastName,
                MobileNumber = _faker.Person.Phone,
                CreatedAt = DateTime.UtcNow
            };
        }
        
        [Fact]
        public async Task Return_Unauthorized_When_GetWalletEndpoint_Is_Called_Without_BearerToken()
        {
            // Act
            var response = await httpClient.GetAsync($"api/Wallet/kmkmk");
            var statusCode = response.StatusCode;

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, statusCode);
        }

        [Theory]
        [InlineData("{ UserId = '', Balance = '1'}")]
        public async Task Return_BadRequest_When_CreatingWallet_Without_UserId(string request)
        {
            // Arrange
            var token = new TokenGenerator().GenerateToken(_user);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            var walletRequest = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8,
                MediaTypeNames.Application.Json);
            
            // Act
            var response = await httpClient.PostAsync("api/wallet", walletRequest);
            var statusCode = response.StatusCode;

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, statusCode);
        }
    }
}