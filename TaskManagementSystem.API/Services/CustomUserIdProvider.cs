using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace TaskManagementSystem.API.Services
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                   ?? connection.User?.Identity?.Name;
        }
    }
}

