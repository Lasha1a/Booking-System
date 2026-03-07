using BookingSystem.Application.DTOs.Auth;
using BookingSystem.Application.Interfaces.Auth;
using BookingSystem.Application.Interfaces.GenericRepo;
using BookingSystem.Domain.Entities;
using BookingSystem.Persistence.Specifications.Auth;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Infrastructure.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IGenericRepository<ServiceProvider> _repository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenGenerator _tokenGenerator;

    public AuthService (IGenericRepository<ServiceProvider> repository, IPasswordHasher passwordHasher, ITokenGenerator tokenGenerator)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
    }

    public async Task RegisterAsync(RegisterUserRequest request)
    {
        //checking if email exists
        var existingProvider = await _repository
             .GetQueryWithSpec(new ServiceProviderByEmailSpec(request.Email))
             .FirstOrDefaultAsync();

        if (existingProvider != null)
        {
            throw new InvalidOperationException("email already exists");
        }

        var passwordHash = _passwordHasher.HashPassword(request.Password);

        //create w encapsulation
        var provider = ServiceProvider.Create(
            request.Name,
            request.Email,
            passwordHash,
            request.Specialty
            );

        await _repository.AddAsync(provider);
        await _repository.SaveChangesAsync();
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var provider = await _repository
            .GetQueryWithSpec(new ServiceProviderByEmailSpec(request.Email))
            .FirstOrDefaultAsync();

        if(provider == null)
        {
            throw new InvalidOperationException("Invalid email or password");
        }

        var isPasswordValid = _passwordHasher.VerifyPassword(request.Password, provider.PasswordHash);
        if (!isPasswordValid)
        {
            throw new InvalidOperationException("Invalid email or password");
        }

        //token 
        var token = _tokenGenerator.GenerateToken(provider.Id, provider.Email, provider.Name);

        return new AuthResponse { Token = token };
    }
}
