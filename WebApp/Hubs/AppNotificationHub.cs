using AppGlobal.Services;
using Microsoft.AspNetCore.SignalR;
using OnaxTools.Dto.Http;
using OnaxTools.Dto.Identity;
using System.Security.Claims;

namespace WebApp.Hubs
{
    public class AppNotificationHub : Hub
    {
        // Store user connections
        private static readonly Dictionary<string, string> UserConnections = new Dictionary<string, string>();
        private readonly IUserProfileService _userProfileService;

        public AppNotificationHub(IUserProfileService userProfileService)
        {
            this._userProfileService = userProfileService;
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }


        #region
        public override async Task OnConnectedAsync()
        {
            // Get user identifier (e.g., from claims)
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var connectionId = Context.ConnectionId;

            // Store connection details
            if (!string.IsNullOrEmpty(userId))
            {
                // Ensure thread-safety
                lock (UserConnections)
                {
                    UserConnections[userId] = connectionId;
                }
            }
            // Log connection
            Console.WriteLine($"User Connected: {userId}, Connection ID: {connectionId}");

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var connectionId = Context.ConnectionId;

            // Remove connection details
            if (!string.IsNullOrEmpty(userId))
            {
                lock (UserConnections)
                {
                    UserConnections.Remove(userId);
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

        // Method to get connection details
        public async Task GetConnectionDetails(string UserIdentifier)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            GenResponse<AppUserIdentity?> altUserdetails = await _userProfileService.FindUserByEmailOrGuid(UserIdentifier);
            var connectionId = Context.ConnectionId;

            // Send connection details back to the client
            await Clients.Caller.SendAsync("ReceiveConnectionDetails", new
            {
                UserId = userId,
                ConnectionId = connectionId,
                ConnectionTime = DateTime.UtcNow
            });
        }
        #endregion
    }
}
