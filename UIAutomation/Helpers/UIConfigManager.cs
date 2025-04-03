using Microsoft.Extensions.Configuration;
using System.IO;
using System;
using UIAutomation.Helpers;

namespace UIAutomation.Helpers
{
    public class UIConfigManager
    {
        private static readonly Lazy<UIConfigManager> _lazyInstance = new(() => new UIConfigManager());
        private readonly IConfiguration _configuration;
        private readonly Logger _logger = Logger.Instance;

        private UIConfigManager()
        {
            try
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

                _configuration = builder.Build();
                _logger.Info("Configuration loaded successfully.");
            }
            catch (Exception ex)
            {
                _logger.Error("Error loading configuration", ex);
                throw;
            }
        }

        public static UIConfigManager Instance => _lazyInstance.Value;

        // Retrieve the BaseUrl from configuration
        public string GetBaseUrl()
        {
            try
            {
                var baseUrl = _configuration["TestSettings:BaseUrl"];
                if (string.IsNullOrEmpty(baseUrl))
                {
                    throw new InvalidOperationException("BaseUrl not found in config");
                }
                _logger.Info($"BaseUrl retrieved: {baseUrl}");
                return baseUrl;
            }
            catch (Exception ex)
            {
                _logger.Error("Error retrieving BaseUrl", ex);
                throw;
            }
        }

        // Retrieve a URL by key from configuration
        public string GetUrl(string key)
        {
            try
            {
                var url = _configuration[$"TestSettings:{key}"];
                if (string.IsNullOrEmpty(url))
                {
                    throw new InvalidOperationException($"URL for {key} not found in config");
                }
                _logger.Info($"URL for {key} retrieved: {url}");
                return url;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error retrieving URL for {key}", ex);
                throw;
            }
        }

        // Retrieve the browser setting from configuration
        public string GetBrowser()
        {
            try
            {
                var browser = _configuration["TestSettings:Browser"] ?? "chromium";
                _logger.Info($"Browser retrieved: {browser}");
                return browser;
            }
            catch (Exception ex)
            {
                _logger.Error("Error retrieving Browser", ex);
                throw;
            }
        }

        // Retrieve the Headless setting from configuration
        public bool GetHeadless()
        {
            try
            {
                bool headless = bool.TryParse(_configuration["TestSettings:Headless"], out bool result) && result;
                _logger.Info($"Headless mode set to: {headless}");
                return headless;
            }
            catch (Exception ex)
            {
                _logger.Error("Error retrieving Headless mode", ex);
                throw;
            }
        }

        // Retrieve the SlowMo setting from configuration
        public int GetSlowMo()
        {
            try
            {
                int slowMo = int.TryParse(_configuration["TestSettings:SlowMo"], out int result) ? result : 0;
                _logger.Info($"SlowMo set to: {slowMo} ms");
                return slowMo;
            }
            catch (Exception ex)
            {
                _logger.Error("Error retrieving SlowMo", ex);
                throw;
            }
        }

        // Retrieve the Timeout setting from configuration
        public int GetTimeout()
        {
            try
            {
                int timeout = int.TryParse(_configuration["TestSettings:Timeout"], out int result) ? result : 30000;
                _logger.Info($"Timeout set to: {timeout} ms");
                return timeout;
            }
            catch (Exception ex)
            {
                _logger.Error("Error retrieving Timeout", ex);
                throw;
            }
        }

        // Check if screenshots should be taken on failure
        public bool TakeScreenshotOnFailure()
        {
            try
            {
                bool takeScreenshot = bool.TryParse(_configuration["TestSettings:Screenshots:TakeOnFailure"], out bool result) && result;
                _logger.Info($"TakeScreenshotOnFailure set to: {takeScreenshot}");
                return takeScreenshot;
            }
            catch (Exception ex)
            {
                _logger.Error("Error retrieving TakeScreenshotOnFailure setting", ex);
                throw;
            }
        }

        // Retrieve the directory path where screenshots are saved
        public string GetScreenshotDirectory()
        {
            try
            {
                var directory = _configuration["TestSettings:Screenshots:Directory"] ?? "screenshots";
                _logger.Info($"Screenshot directory set to: {directory}");
                return directory;
            }
            catch (Exception ex)
            {
                _logger.Error("Error retrieving Screenshot Directory", ex);
                throw;
            }
        }

        // Retrieve the username for a specific user type from configuration
        public string GetUsername(string userType = "StandardUser")
        {
            try
            {
                var username = _configuration[$"TestSettings:Credentials:{userType}:Username"];
                if (string.IsNullOrEmpty(username))
                {
                    throw new InvalidOperationException($"Username for {userType} not found in config");
                }
                _logger.Info($"Username for {userType} retrieved: {username}");
                return username;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error retrieving Username for {userType}", ex);
                throw;
            }
        }

        // Retrieve the password for a specific user type from configuration
        public string GetPassword(string userType = "StandardUser")
        {
            try
            {
                var password = _configuration[$"TestSettings:Credentials:{userType}:Password"];
                if (string.IsNullOrEmpty(password))
                {
                    throw new InvalidOperationException($"Password for {userType} not found in config");
                }
                _logger.Info($"Password for {userType} retrieved.");
                return password;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error retrieving Password for {userType}", ex);
                throw;
            }
        }
    }
}
