using BookingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Persistence.Specifications.Appointments;

public class RecurringChildAppointmentsSpec : BaseSpecification<Appointment>
{
    public RecurringChildAppointmentsSpec(Guid parentAppointmentId)
        : base(a => a.ParentAppointmentId == parentAppointmentId)
    {
    }
}


//Used when **cancelling a recurring series** — finds all child appointments that belong to a parent appointment