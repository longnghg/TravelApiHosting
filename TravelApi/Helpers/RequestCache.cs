using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravelApi.Configs;

namespace TravelApi.Helpers
{
    // vercel.com
    public static class RequestCache
    {
        private static IHttpContextAccessor _httpAccessor = GetConfigItems.HttpContextAccessor;

        public static T Get<T>(string key)
        {
            T data = default;
            if (string.IsNullOrEmpty(key))
            {
                return data;
            }
            data = (T)_httpAccessor.HttpContext.Items[key];
            return data;
        }
        public static bool Set<T>(T data, string key)
        {
            if (data == null || string.IsNullOrEmpty(key))
            {
                return false;
            }
            _httpAccessor.HttpContext.Items[key] = data;
            return true;
        }

        public static bool Update<T>(T data, string key)
        {
            if (data == null || string.IsNullOrEmpty(key))
            {
                return false;
            }
            Remove(key);
            return Set(data, key);
        }

        public static void Remove(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                _httpAccessor.HttpContext.Items.Remove(key);
            }
        }

      
    }
}
