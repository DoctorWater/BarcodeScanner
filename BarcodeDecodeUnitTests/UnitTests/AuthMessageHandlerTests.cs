using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BarcodeDecodeBackend.Services.Processing;
using BarcodeDecodeLib.Models.Dtos.Messages.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;

namespace BarcodeDecodeUnitTests.UnitTests;

[TestFixture]
public class AuthMessageHandlerTests
{
    private Mock<UserManager<IdentityUser>> _userManagerMock;
    private IConfiguration _configuration;
    private AuthMessageHandler _handler;

    [SetUp]
    public void SetUp()
    {
        var userStoreMock = new Mock<IUserStore<IdentityUser>>();
        _userManagerMock = new Mock<UserManager<IdentityUser>>(
            userStoreMock.Object,
            null, null, null, null, null, null, null, null
        );
        
        var inMemorySettings = new Dictionary<string, string>
        {
            ["JwtSettings:Key"] = "0123456789ABCDEF0123456789ABCDEF",
            ["JwtSettings:Issuer"] = "TestIssuer",
            ["JwtSettings:Audience"] = "TestAudience",
            ["JwtSettings:DurationInMinutes"] = "60"
        };
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
        
        _handler = new AuthMessageHandler(_userManagerMock.Object, _configuration);
    }

    [Test]
    public async Task Login_ShouldReturnToken_WhenCredentialsAreValid()
    {
        var dto = new LoginDto { Username = "alice", Password = "pwd" };
        var user = new IdentityUser { UserName = "alice" };
        var roles = new List<string> { "Admin", "User" };

        _userManagerMock
            .Setup(m => m.FindByNameAsync("alice"))
            .ReturnsAsync(user);
        _userManagerMock
            .Setup(m => m.CheckPasswordAsync(user, "pwd"))
            .ReturnsAsync(true);
        _userManagerMock
            .Setup(m => m.GetRolesAsync(user))
            .ReturnsAsync(roles);

        var tokenString = await _handler.Login(dto);

        Assert.That(tokenString, Is.Not.Null.And.Not.Empty);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(tokenString);
        Assert.That(jwt.Issuer, Is.EqualTo(_configuration["JwtSettings:Issuer"]));
        Assert.That(jwt.Audiences, Does.Contain(_configuration["JwtSettings:Audience"]));
        Assert.That(jwt.Subject, Is.EqualTo("alice"));

        foreach (var role in roles)
        {
            Assert.That(jwt.Claims, Has.Some.Matches<Claim>(c =>
                c.Type == ClaimTypes.Role && c.Value == role));
        }
    }

    [Test]
    public async Task Login_ShouldReturnNull_WhenUserNotFound()
    {
        _userManagerMock
            .Setup(m => m.FindByNameAsync("bob"))
            .ReturnsAsync((IdentityUser?)null);

        var result = await _handler.Login(new LoginDto { Username = "bob", Password = "any" });

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Login_ShouldReturnNull_WhenPasswordInvalid()
    {
        var user = new IdentityUser { UserName = "carol" };
        _userManagerMock
            .Setup(m => m.FindByNameAsync("carol"))
            .ReturnsAsync(user);
        _userManagerMock
            .Setup(m => m.CheckPasswordAsync(user, "wrong"))
            .ReturnsAsync(false);

        var result = await _handler.Login(new LoginDto { Username = "carol", Password = "wrong" });

        Assert.That(result, Is.Null);
    }
}