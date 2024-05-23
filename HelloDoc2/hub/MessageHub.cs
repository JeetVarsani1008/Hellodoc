using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace HelloDoc2.hub
{
    public class MessageHub : Hub
    {
        private static ConcurrentBag<string> _messages = new ConcurrentBag<string>();
        private static Dictionary<int, List<string>> userConnectionMap = new Dictionary<int, List<string>>();
        private static string userConnectionMapLocker = string.Empty;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MessageHub(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }


        public async void SendMessage(string userId, string Message)
        {
            await Clients.All.SendAsync("ReceiveMessage", userId, Message);
        }

        public override async Task OnConnectedAsync()
        {
            if (_httpContextAccessor.HttpContext?.Session.GetInt32("AspId") != null)
            {
                KeepUserConnection(Context.ConnectionId);
            }

            if (_httpContextAccessor.HttpContext?.Session.GetInt32("AdminId") != null)
            {
                await AddToGroup("AllAdmins");
            }

            if (_httpContextAccessor.HttpContext?.Session.GetInt32("UserId") != null)
            {
                await AddToGroup("AllUsers");
            }
            await base.OnConnectedAsync();
        }

        public static List<string> GetUserConnections(int userId)
        {
            var conn = new List<string>();
            lock (userConnectionMapLocker)
            {
                if (userConnectionMap.ContainsKey(userId))
                {
                    conn = userConnectionMap[userId];
                }
            }
            return conn;
        }

        public void KeepUserConnection(string connectionId)
        {
            lock (userConnectionMapLocker)
            {
                int userId = _httpContextAccessor.HttpContext?.Session.GetInt32("AspId") ?? 0;
                if (!userConnectionMap.ContainsKey(userId))
                {
                    userConnectionMap[userId] = new List<string>();
                }
                userConnectionMap[userId].Add(connectionId);
            }
        }

        public async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

    }
}
