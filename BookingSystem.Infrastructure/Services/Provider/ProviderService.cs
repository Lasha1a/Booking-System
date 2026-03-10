using BookingSystem.Application.DTOs.Provider;
using BookingSystem.Application.Interfaces.GenericRepo;
using BookingSystem.Application.Interfaces.Provider;
using BookingSystem.Application.Interfaces.RedisCache;
using BookingSystem.Domain.Entities;
using BookingSystem.Infrastructure.Services.RedisCache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Infrastructure.Services.Provider;

public class ProviderService : IProviderService
{
    private readonly IGenericRepository<ServiceProvider> _repository;
    private readonly ICacheService _cacheService;


    public ProviderService(IGenericRepository<ServiceProvider> repository, ICacheService cacheService)
    {
        _repository = repository;
        _cacheService = cacheService;
    }

    public async Task<ProviderResponse> GetProfileAsync(Guid providerId)
    {


        var cacheKey  = $"provider:{providerId}:profile";

        //check cache
        var cached = await _cacheService.GetAsync<ProviderResponse>(cacheKey);
        if (cached != null)
        {
            return cached;
        }
        var provider = await _repository.GetByIdAsync(providerId);

        if (provider == null)
        {
            throw new KeyNotFoundException("Provider not found");
        }

        var response = new ProviderResponse
        {
            Id = provider.Id,
            Name = provider.Name,
            Email = provider.Email,
            Specialty = provider.Specialty,
            IsActive = provider.IsActive
        };

        // Save to cache
        await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(10));

        return response;
    }

    public async Task UpdateProfileAsync(Guid providerId, UpdateProviderRequest request)
    {
        var provider = await _repository.GetByIdAsync(providerId);

        if (provider == null)
            throw new KeyNotFoundException("Provider not found");

        //with encapsulation
        provider.UpdateProfile(request.Name, request.Specialty);

        _repository.Update(provider);
        await _repository.SaveChangesAsync();

        await _cacheService.RemoveAsync($"provider:{providerId}:profile");
    }

    public async Task DeleteProfileAsync(Guid providerId)
    {
        var provider = await _repository.GetByIdAsync(providerId);

        if (provider == null)
            throw new KeyNotFoundException("Provider not found");

        _repository.Delete(provider);
        await _repository.SaveChangesAsync();

        await _cacheService.RemoveAsync($"provider:{providerId}:profile");
    }
}
