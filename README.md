Booking System API
A REST API for appointment booking built with .NET 9 and Clean Architecture. Service providers can manage their profiles, working hours, and blocked times, while customers can search available slots and book appointments.
Tech Stack

.NET 9 — Web API
PostgreSQL — Database
Redis — Caching
Entity Framework Core — ORM
JWT — Authentication
MailKit — Email notifications
FluentValidation — Request validation
Docker — Containerization

Architecture
The project follows Clean Architecture principles with 5 layers.
Features

Auth — Register and login with JWT authentication
Provider Management — Profile, working hours, blocked times/vacations
Appointment Booking — Search available slots, book, cancel, reschedule
Recurring Appointments — Weekly and monthly recurring bookings
Conflict Detection — Prevents double booking
Email Notifications — Confirmation and cancellation emails with retry logic
Background Service — Auto cleanup of old completed/cancelled appointments
Redis Caching — Caches read endpoints for better performance

Getting Started
Prerequisites

.NET 9 SDK
Docker Desktop

1. Clone the repository
bashgit clone https://github.com/Lasha1a/Booking-System.git
cd Booking-System
2. Configure credentials
Open appsettings.json and fill in your credentials:
json{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=booking_db;Username=postgres;Password=postgres"
  },
  "Redis": {
    "ConnectionString": "localhost:6379",
    "InstanceName": "BookingSystem:"
  },
  "JwtSettings": {
    "SecretKey": "your-secret-key",
    "Issuer": "BookingSystem",
    "Audience": "BookingSystem",
    "ExpiryMinutes": 60
  },
  "SmtpSettings": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "Username": "your-email@gmail.com",
    "Password": "your-app-password",
    "FromEmail": "your-email@gmail.com",
    "FromName": "Booking System",
    "MaxRetryAttempts": 3,
    "RetryDelaySeconds": 2
  }
}
3. Start Docker containers
bashdocker-compose up -d
This starts PostgreSQL and Redis containers. Verify they're running:
bashdocker ps
4. Run Migrations
Option 1 — Package Manager Console (Visual Studio):
Add-Migration InitialCreate -Project BookingSystem.Persistence -StartupProject BookingSystem
Update-Database
Option 2 — .NET CLI:
bashdotnet ef migrations add InitialCreate --project BookingSystem.Persistence --startup-project BookingSystem
dotnet ef database update --project BookingSystem.Persistence --startup-project BookingSystem
5. Run the API
bashdotnet run --project BookingSystem
The API will be available at https://localhost:7187. Open Scalar UI at https://localhost:7187/scalar/v1.
Authentication
Protected endpoints require a Bearer token in the Authorization header:
Authorization: Bearer your_jwt_token
Get the token by logging in via /api/Auth/log-in.
Appointment Duration
Appointments support the following durations: 15, 30, 45, or 60 minutes
Recurring Appointments
Set isRecurring: true and recurrenceRule to either:

"weekly" — repeats every week for 4 weeks
"monthly" — repeats every month for 3 months
