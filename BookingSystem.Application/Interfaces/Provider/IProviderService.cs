using BookingSystem.Application.DTOs.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Application.Interfaces.Provider;

public interface IProviderService
{
    Task<ProviderResponse> GetProfileAsync(Guid providerId);
    Task UpdateProfileAsync(Guid providerId, UpdateProviderRequest request);
    Task DeleteProfileAsync(Guid providerId);
}
