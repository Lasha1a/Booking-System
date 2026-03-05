using BookingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Application.Interfaces.Auth;

public interface ITokenGenerator
{
    string GenerateToken(Guid providerId, string email, string name);
}
