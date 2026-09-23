using SimpleCommerce.Application.Services;
using SimpleCommerce.Domain.Enums;
using SimpleCommerce.UnitTests.Application;
using Xunit;

namespace SimpleCommerce.UnitTests.Application;

public class AuthServiceTests
{
    private readonly FakeUserRepository _userRepository = new();
    private readonly FakePasswordHasher _passwordHasher = new();
    private readonly FakeTokenService _tokenService = new();
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _service = new AuthService(_userRepository, _passwordHasher, _tokenService);
    }

    [Fact]
    public async Task Register_WithValidData_CreatesCustomerUser()
    {
        var result = await _service.RegisterAsync(new("ali@test.com", "Secret@123", "Ali Rezaei", "Customer"));

        Assert.True(result.Succeeded);
        Assert.Equal("ali@test.com", result.Value!.Email);
        Assert.Equal(UserRole.Customer, Enum.Parse<UserRole>(result.Value.Role));
        Assert.Single(_userRepository.Users);
        Assert.Equal(1, _userRepository.SaveCount);
    }

    [Fact]
    public async Task Register_WithAdminRole_CreatesAdminUser()
    {
        var result = await _service.RegisterAsync(new("boss@test.com", "Secret@123", "The Boss", "Admin"));

        Assert.True(result.Succeeded);
        Assert.Equal(UserRole.Admin, Enum.Parse<UserRole>(result.Value!.Role));
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_FailsWithoutSaving()
    {
        await _service.RegisterAsync(new("ali@test.com", "Secret@123", "Ali", "Customer"));

        var result = await _service.RegisterAsync(new("ALI@test.com", "Other@123", "Ali Two", "Customer"));

        Assert.False(result.Succeeded);
        Assert.Contains("already exists", result.Error);
        Assert.Equal(1, _userRepository.SaveCount); // no second save
    }

    [Fact]
    public async Task Register_WithInvalidRole_Fails()
    {
        var result = await _service.RegisterAsync(new("x@test.com", "Secret@123", "X", "SuperAdmin"));

        Assert.False(result.Succeeded);
        Assert.Contains("Role", result.Error);
    }

    [Fact]
    public async Task Register_WithShortPassword_Fails()
    {
        var result = await _service.RegisterAsync(new("x@test.com", "123", "X", "Customer"));

        Assert.False(result.Succeeded);
        Assert.Contains("6 characters", result.Error);
    }

    [Fact]
    public async Task Register_StoresHashedPasswordNotPlainText()
    {
        var result = await _service.RegisterAsync(new("ali@test.com", "Secret@123", "Ali", "Customer"));

        var user = _userRepository.Users.Single();
        Assert.NotEqual("Secret@123", user.PasswordHash);
        Assert.Equal(_passwordHasher.HashOf("Secret@123"), user.PasswordHash);
    }

    [Fact]
    public async Task Login_WithCorrectCredentials_ReturnsTokenAndRole()
    {
        await _service.RegisterAsync(new("ali@test.com", "Secret@123", "Ali", "Admin"));

        var result = await _service.LoginAsync(new("ali@test.com", "Secret@123"));

        Assert.True(result.Succeeded);
        Assert.Contains("Admin", result.Value!.Token);
        Assert.Equal(UserRole.Admin, Enum.Parse<UserRole>(result.Value.Role));
    }

    [Fact]
    public async Task Login_WithWrongPassword_Fails()
    {
        await _service.RegisterAsync(new("ali@test.com", "Secret@123", "Ali", "Customer"));

        var result = await _service.LoginAsync(new("ali@test.com", "Wrong@123"));

        Assert.False(result.Succeeded);
        Assert.Equal("Invalid email or password.", result.Error); // no user enumeration
    }

    [Fact]
    public async Task Login_WithUnknownEmail_Fails()
    {
        var result = await _service.LoginAsync(new("ghost@test.com", "Whatever@123"));

        Assert.False(result.Succeeded);
        Assert.Contains("Invalid email or password", result.Error);
    }
}
