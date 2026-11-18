using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbAccessaLayer.Helpers
{
    public class CustomerUserIdProvider:IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            var userId = connection.User?.FindFirst("UserId")?.Value;
            Console.WriteLine($"SignalR UserId resolved: {userId}");
            return userId;
        }
    }
}
