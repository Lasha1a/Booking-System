using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Application.Interfaces.Email;

public interface IEmailService
{
    Task SendEmailAsync(string email,string ToName, string subject,string body);
}
