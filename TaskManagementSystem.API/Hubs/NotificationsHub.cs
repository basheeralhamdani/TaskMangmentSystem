using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace TaskManagementSystem.API.Hubs
{
    [Authorize]
    public class NotificationsHub : Hub
    {
        public async Task JoinTaskGroup(string taskId)
        {
            if (!string.IsNullOrWhiteSpace(taskId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"task:{taskId}");
            }
        }

        public async Task LeaveTaskGroup(string taskId)
        {
            if (!string.IsNullOrWhiteSpace(taskId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"task:{taskId}");
            }
        }
    }
}

