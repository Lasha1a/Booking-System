using BookingSystem.Application.DTOs.WorkingHours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Application.Interfaces.WorkingHours;

public interface IWorkingHoursService
{
    Task AddWorkingHoursAsync(Guid providerId, AddWorkingHoursRequest request);
    Task<IReadOnlyList<WorkingHoursResponse>> GetWorkingHoursAsync(Guid providerId);
    Task UpdateWorkingHoursAsync(Guid providerId, Guid workingHoursId, UpdateWorkingHoursRequest request);
    Task DeleteWorkingHoursAsync(Guid providerId, Guid workingHoursId);
}
