using Microsoft.Playwright;
using System;
using System.IO;
using System.Threading.Tasks;
using UIAutomation.Helpers;

namespace UIAutomation.Drivers
{
    public class PlaywrightManager
    {
        private static readonly Logger _logger = Logger.Instance;
        private static readonly UIConfigManager _config = UIConfigManager.Instance;

        private IPlaywright? _playwright;
        private IBrowser? _browser;
        private IBrowserContext? _context;
        private IPage? _page;

        // Initialize Playwright and browser context
        public async Task InitializeAsync()
        {
            try
            {
                _logger.Info("Initializing Playwright");

                // Create Playwright instance
                _playwright = await Playwright.CreateAsync();

                var browserType = _config.GetBrowser().ToLower() switch
                {
                    "firefox" => _playwright.Firefox,
                    "webkit" => _playwright.Webkit,
                    _ => _playwright.Chromium
                };

                _logger.Info($"Launching {_config.GetBrowser()} browser");

                // Launch the browser with configuration options
                _browser = await browserType.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = _config.GetHeadless(),
                    SlowMo = _config.GetSlowMo()
                });

                _logger.Info("Creating browser context");

                // Create browser context with viewport settings
                _context = await _browser.NewContextAsync(new BrowserNewContextOptions
                {
                    ViewportSize = new ViewportSize
                    {
                        Width = 1280,
                        Height = 720
                    }
                });

                _logger.Info("Creating new page");

                // Create a new page and set default timeout
                _page = await _context.NewPageAsync();
                _page.SetDefaultTimeout(_config.GetTimeout());

                // Setup screenshot directory if needed
                if (_config.TakeScreenshotOnFailure())
                {
                    string screenshotDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _config.GetScreenshotDirectory());
                    if (!Directory.Exists(screenshotDir))
                    {
                        Directory.CreateDirectory(screenshotDir);
                    }
                }

                _logger.Info("Playwright initialized successfully");
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to initialize Playwright", ex);
                throw;
            }
        }

        // Retrieve the current page
        public IPage GetPage()
        {
            if (_page == null)
            {
                throw new InvalidOperationException("Playwright has not been initialized. Call InitializeAsync() first.");
            }

            return _page;
        }

        // Navigate to the provided URL
        public async Task NavigateToUrl(string url)
        {
            try
            {
                _logger.Info($"Navigating to {url}");

                if (_page == null)
                {
                    throw new InvalidOperationException("Page not initialized");
                }

                await _page.GotoAsync(url);

                _logger.Info($"Successfully navigated to {url}");
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to navigate to {url}", ex);
                throw;
            }
        }

        // Take a screenshot and save it to the configured directory
        public async Task TakeScreenshotAsync(string name)
        {
            try
            {
                if (_page == null)
                {
                    throw new InvalidOperationException("Page not initialized");
                }

                string screenshotDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _config.GetScreenshotDirectory());
                string fileName = $"{name}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                string filePath = Path.Combine(screenshotDir, fileName);

                // Capture the screenshot
                await _page.ScreenshotAsync(new PageScreenshotOptions
                {
                    Path = filePath,
                    FullPage = true
                });

                _logger.Info($"Screenshot saved to {filePath}");
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to take screenshot", ex);
            }
        }

        // Close Playwright resources and clean up
        public async Task CloseAsync()
        {
            try
            {
                _logger.Info("Closing Playwright resources");

                // Close browser context and browser
                if (_context != null)
                {
                    await _context.CloseAsync();
                }

                if (_browser != null)
                {
                    await _browser.CloseAsync();
                }

                // Dispose of Playwright instance
                _playwright?.Dispose();

                _logger.Info("Playwright resources closed successfully");
            }
            catch (Exception ex)
            {
                _logger.Error("Error while closing Playwright resources", ex);
                throw;
            }
        }
    }
}
