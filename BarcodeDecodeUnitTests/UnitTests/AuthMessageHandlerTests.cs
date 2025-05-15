using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BarcodeDecodeBackend.Services.Processing;
using BarcodeDecodeLib.Models.Dtos.Configs;
using BarcodeDecodeLib.Models.Dtos.Messages.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;

namespace BarcodeDecodeUnitTests.UnitTests;

[TestFixture]
public class AuthMessageHandlerTests
{
    private Mock<UserManager<IdentityUser>> _userManagerMock;
    private IOptions<JwtSettings> _jwtOptions;
    private AuthMessageHandler _handler;

    [SetUp]
    public void SetUp()
    {
        var userStoreMock = new Mock<IUserStore<IdentityUser>>();
        _userManagerMock = new Mock<UserManager<IdentityUser>>(
            userStoreMock.Object,
            null, null, null, null, null, null, null, null
        );
        
        _jwtOptions = Options.Create(new JwtSettings()
        {
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            DurationInMinutes = 60,
            Key = "0123456789ABCDEF0123456789ABCDEF"
        });
        
        _handler = new AuthMessageHandler(_userManagerMock.Object, _jwtOptions);
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

        Assert.That(tokenString, Is.Not.Null);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(tokenString.Token);
        Assert.That(jwt.Issuer, Is.EqualTo(_jwtOptions.Value.Issuer));
        Assert.That(jwt.Audiences, Does.Contain(_jwtOptions.Value.Audience));

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