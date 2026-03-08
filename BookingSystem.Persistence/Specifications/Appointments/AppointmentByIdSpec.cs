using BookingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Persistence.Specifications.Appointments;

//get appointment by id for cannceletion/update
public class AppointmentByIdSpec : BaseSpecification<Appointment>
{
    public AppointmentByIdSpec(Guid appointmentId)
        : base(a => a.Id == appointmentId)
    {
    }
}
