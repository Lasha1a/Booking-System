using BookingSystem.Domain.Entities;
using BookingSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Persistence.Specifications.Appointments;


//Used when searching available slots — gets all booked appointments for a specific date so we know which slots are taken
public class BookedAppointmentsByDateSpec : BaseSpecification<Appointment>
{
    public BookedAppointmentsByDateSpec(Guid providerId, DateOnly date)
        : base(a => a.ProviderId == providerId
            && a.AppointmentDate.Date == date.ToDateTime(TimeOnly.MinValue).Date
            && a.Status != AppointmentStatus.Cancelled)
    {
    }
}
