using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIAutomation.Helpers
{
    public class PlaywrightFixture : IDisposable
    {
        private bool _disposed = false;
        private readonly Logger _logger = Logger.Instance;
        private readonly APIConfigManager _config = APIConfigManager.Instance;

        public IPlaywright Playwright { get; }
        public IAPIRequestContext RequestContext { get; }

        public PlaywrightFixture()
        {
            _logger.Info("Initializing Playwright and API request context");
            Playwright = Task.Run(async () => await Microsoft.Playwright.Playwright.CreateAsync()).Result;
            RequestContext = Task.Run(async () => await CreateAPIRequestContextAsync()).Result;
        }

        private async Task<IAPIRequestContext> CreateAPIRequestContextAsync()
        {
            var baseUrl = _config.GetBaseUrl();
            _logger.Info($"Creating API request context with base URL: {baseUrl}");

            return await Playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions
            {
                BaseURL = baseUrl,
                IgnoreHTTPSErrors = true,
                ExtraHTTPHeaders = new Dictionary<string, string>
                {
                    { "Accept", "application/json" },
                    { "Content-Type", "application/json" }
                },
                Timeout = _config.GetRequestTimeout()
            });
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _logger.Info("Disposing Playwright resources");
                    Task.Run(async () => await RequestContext.DisposeAsync()).Wait();
                    Playwright.Dispose();
                }

                _disposed = true;
            }
        }

        ~PlaywrightFixture()
        {
            Dispose(false);
        }
    }
}
