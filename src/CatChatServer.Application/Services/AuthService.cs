using CatChatServer.Domain.Entities;
using CatChatServer.Domain.Enums;
using CatChatServer.Domain.Interfaces;
using CatChatServer.Domain.Models.Auth;
using CatChatServer.Domain.Repositories;

namespace CatChatServer.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse> Login(LoginRequest request)
    {
        User user = await _userRepository.GetUserByEmailAsync(request.Email);
        if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new GraphQLException("User does not exist");
        }
        
        AuthResponse authResponse = GenerateTokens(user);
        
        user.RefreshToken = authResponse.RefreshToken;
        
        await _userRepository.UpdateAsync(user);

        return authResponse;
    }

    public async Task<AuthResponse> Register(RegisterRequest request)
    {
        // Check if username already exists
        IEnumerable<User> existingUsername = await _userRepository
            .FindAsync(u => u.Username == request.Username);
        if (existingUsername.Any())
        {
            throw new Exception("Username is already taken.");
        }

        // Check if email already exists
        IEnumerable<User> existingEmail = await _userRepository
            .FindAsync(u => u.Email == request.Email);
        if (existingEmail.Any())
        {
            throw new Exception("Email is already registered.");
        }

        // Hash password
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // Create user
        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = passwordHash,
            LastLoginAt = DateTime.UtcNow,
            Status = EUserStatus.Online
        };

        // Generate tokens
        AuthResponse authResponse = GenerateTokens(user);

        // Set refresh token details
        user.RefreshToken = authResponse.RefreshToken;

        // Insert user
        await _userRepository.AddAsync(user);

        return authResponse;
    }

    public async Task<AuthResponse> RefreshToken(RefreshTokenRequest request)
    {
        IEnumerable<User> existingUsers = await _userRepository
            .FindAsync(u => u.RefreshToken == request.RefreshToken);

        User user = existingUsers.FirstOrDefault();

        if (user == null || _tokenService.ValidateTokenExpired(request.RefreshToken))
        {
            throw new Exception("Refresh token was expired");
        }

        AuthResponse authResponse = GenerateTokens(user);

        user.RefreshToken = authResponse.RefreshToken;

        await _userRepository.UpdateAsync(user);

        return authResponse;
    }

    private AuthResponse GenerateTokens(User user)
    {
        string accessToken = _tokenService.GenerateAccessToken(user);
        string refreshToken = _tokenService.GenerateRefreshToken();

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
        };
    }
}
