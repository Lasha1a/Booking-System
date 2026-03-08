using BookingSystem.Application.DTOs.Provider;
using BookingSystem.Application.Interfaces.GenericRepo;
using BookingSystem.Application.Interfaces.Provider;
using BookingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Infrastructure.Services.Provider;

public class ProviderService : IProviderService
{
    private readonly IGenericRepository<ServiceProvider> _repository;

    public ProviderService(IGenericRepository<ServiceProvider> repository)
    {
        _repository = repository;
    }

    public async Task<ProviderResponse> GetProfileAsync(Guid providerId)
    {
        var provider = await _repository.GetByIdAsync(providerId);

        if (provider == null)
        {
            throw new KeyNotFoundException("Provider not found");
        }

        return new ProviderResponse
        {
            Id = provider.Id,
            Name = provider.Name,
            Email = provider.Email,
            Specialty = provider.Specialty,
            IsActive = provider.IsActive
        };
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
    }

    public async Task DeleteProfileAsync(Guid providerId)
    {
        var provider = await _repository.GetByIdAsync(providerId);

        if (provider == null)
            throw new KeyNotFoundException("Provider not found");

        _repository.Delete(provider);
        await _repository.SaveChangesAsync();
    }
}
