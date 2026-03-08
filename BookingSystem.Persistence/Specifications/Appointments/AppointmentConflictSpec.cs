using BookingSystem.Domain.Entities;
using BookingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Persistence.Specifications.Appointments;


//This is the** conflict detection spec,  checks if a time slot is already booked.
public class AppointmentConflictSpec : BaseSpecification<Appointment>
{
    public AppointmentConflictSpec(Guid providerId, DateTime appointmentDate, TimeOnly startTime, TimeOnly endTime)
    : base(a => a.ProviderId == providerId
        && a.AppointmentDate.Date == appointmentDate.Date
        && a.Status != AppointmentStatus.Cancelled
        && a.StartTime < endTime
        && a.EndTime > startTime)
    {
    }
}
