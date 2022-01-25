using System;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using FastPaceTransferTest2022.Api.Models.Requests;
using FastPaceTransferTest2022.Api.Models.Responses;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace FastPaceTransferTest2022.Api.Tests
{
    public class AuthControllerShould : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient httpClient;
        private readonly Faker _faker;
        
        public AuthControllerShould(WebApplicationFactory<Startup> factory)
        {
            httpClient = factory.CreateClient();
            _faker = new Faker();
        }
        
        [Fact]
        public void Check_If_GenerateToken_DoesNotReturnNull_On_Successful_Login()
        {
            // Arrange
            var faker = new Faker();
            var user = new UserResponse
            {
                Id = Guid.NewGuid().ToString("N"),
                EmailAddress = faker.Person.Email,
                FirstName = faker.Person.FirstName,
                LastName = faker.Person.LastName,
                MobileNumber = faker.Person.Phone,
                CreatedAt = DateTime.UtcNow
            };
            
            // Act
            var token = new TokenGenerator().GenerateToken(user);
            
            // Assert
            Assert.NotNull(token);
        }
        
        [Fact]
        public async Task Return_BadRequest_When_User_Registers_With_NonMatchingPassword()
        {
            // Arrange
            var registerRequest = new CreateUserRequest
            {
                FirstName = _faker.Person.FirstName,
                LastName = _faker.Person.LastName,
                EmailAddress = _faker.Person.Email,
                MobileNumber = _faker.Person.Phone,
                Password = _faker.Random.Words(2),
                ConfirmPassword = _faker.Random.Words(3),
            };

            var request = new StringContent(JsonConvert.SerializeObject(registerRequest), Encoding.UTF8,
                MediaTypeNames.Application.Json);
            
            // Act
            var response = await httpClient.PostAsync("api/Auth/register", request);
            var statusCode = response.StatusCode;

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, statusCode);
        }
        
        [Theory]
        [InlineData("{ FirstName = '', LastName = 'Mensah', EmailAddress = 'paul@gmail.com'}")]
        [InlineData("{ FirstName = 'Paul', LastName = 'Mensah', EmailAddress = 'paul@gmail.com'}")]
        [InlineData("{ FirstName = 'Paul', LastName = 'Mensah', EmailAddress = 'paul@gmail.com', MobileNumber = '0241234567', Password = 'Password'}")]
        public async Task Return_BadRequest_When_User_Registers_Without_RequiredFields(string request)
        {
            // Arrange
            var createAccountRequest = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8,
                MediaTypeNames.Application.Json);
            
            // Act
            var response = await httpClient.PostAsync("api/Auth/register", createAccountRequest);
            var statusCode = response.StatusCode;

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, statusCode);
        }
    }
}