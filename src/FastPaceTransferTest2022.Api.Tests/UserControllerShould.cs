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
    public class UserControllerShould : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient httpClient;
        private readonly UserResponse _user;
        private readonly Faker _faker;
        
        public UserControllerShould(WebApplicationFactory<Startup> factory)
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
        public async Task Return_Unauthorized_When_GetAllUsersEndpoint_Is_Called_Without_BearerToken()
        {
            // Act
            var response = await httpClient.GetAsync($"api/User");
            var statusCode = response.StatusCode;

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, statusCode);
        }
        
        [Theory]
        [InlineData("{ FirstName = '', LastName = 'Mensah', EmailAddress = 'paul@gmail.com'}")]
        [InlineData("{ FirstName = 'Paul', LastName = 'Mensah', EmailAddress = 'paul@gmail.com'}")]
        [InlineData("{ FirstName = 'Paul', LastName = '', EmailAddress = 'paul@gmail.com', MobileNumber = '0241234567'}")]
        public async Task Return_BadRequest_When_CreatingUser_Without_RequiredFields(string request)
        {
            // Arrange
            var token = new TokenGenerator().GenerateToken(_user);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            var createAccountRequest = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8,
                MediaTypeNames.Application.Json);
            
            // Act
            var response = await httpClient.PostAsync("api/user", createAccountRequest);
            var statusCode = response.StatusCode;

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, statusCode);
        }
        
        [Fact]
        public async Task Return_200Response_When_GetAllUsersEndpoint_Is_Called_BearerToken()
        {
            // Arrange
            var token = new TokenGenerator().GenerateToken(_user);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            // Act
            var response = await httpClient.GetAsync($"api/User");
            var statusCode = response.StatusCode;

            // Assert
            Assert.Equal(HttpStatusCode.OK, statusCode);
        }
    }
}