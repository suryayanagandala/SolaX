using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DbAccessaLayer.Helpers
{
    public static class MessageHelper
    {
        private static Dictionary<string, Dictionary<string, string>> _messages;

        public static void Initialize(IConfiguration _config)
        {
            _messages = _config.Get<Dictionary<string, Dictionary<string, string>>>();
        }

        public static string Get(string category, string key)
        {
            if (_messages == null)
                return "Messages not initialized";

            if (_messages.TryGetValue(category, out var group) &&
                group.TryGetValue(key, out var value))
                return value;

            return $"Message not found: {category}.{key}";
        }
    }
}
