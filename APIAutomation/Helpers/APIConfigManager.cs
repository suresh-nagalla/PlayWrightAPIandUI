// File: APIAutomation/Helpers/APIConfigManager.cs
using Microsoft.Extensions.Configuration;
using System.IO;
using System;

namespace APIAutomation.Helpers
{
    public class APIConfigManager
    {
        private static readonly Lazy<APIConfigManager> _lazyInstance = new(() => new APIConfigManager());
        private readonly IConfiguration _configuration;

        private APIConfigManager()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            _configuration = builder.Build();
        }

        public static APIConfigManager Instance => _lazyInstance.Value;

        public string GetBaseUrl()
        {
            return _configuration["TestSettings:BaseUrl"] ?? throw new InvalidOperationException("BaseUrl not found in config");
        }

        public int GetRequestTimeout()
        {
            return int.TryParse(_configuration["TestSettings:RequestTimeout"], out int result) ? result : 30000;
        }

        public int GetRetryCount()
        {
            return int.TryParse(_configuration["TestSettings:RetryCount"], out int result) ? result : 3;
        }

        public int GetRetryDelayMilliseconds()
        {
            return int.TryParse(_configuration["TestSettings:RetryDelayMilliseconds"], out int result) ? result : 1000;
        }
    }
}
