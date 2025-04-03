using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UIAutomation.Helpers;

namespace UIAutomation.Pages
{
    public class PlaywrightPageBase
    {
        public IPage _page;
        private const int MaxRetries = 3;
        private const int RetryDelayInMillis = 1000;
        private readonly Logger _logger = Logger.Instance;

        public PlaywrightPageBase(IPage page)
        {
            _page = page ?? throw new ArgumentNullException(nameof(page));
        }

        #region Navigation

        // Navigate to a specific page URL
        public async Task NavigateToPageAsync(string url)
        {
            _logger.Info($"Navigating to URL: {url}");
            await _page.GotoAsync(url);
            _logger.Info($"Successfully navigated to {url}");
        }

        #endregion

        #region Element Interaction

        // Wait for an element to be visible
        public async Task WaitForElementAsync(string selector, int timeoutInMillis = 20000)
        {
            _logger.Info($"Waiting for element with selector '{selector}' to be visible for {timeoutInMillis} ms.");
            try
            {
                await _page.Locator(selector).WaitForAsync(new LocatorWaitForOptions { Timeout = timeoutInMillis });
                _logger.Info($"Element with selector '{selector}' is now visible.");
            }
            catch (TimeoutException)
            {
                _logger.Error($"Element with selector '{selector}' not visible after {timeoutInMillis} ms.");
                throw new TimeoutException($"Element with selector {selector} not visible after {timeoutInMillis} ms.");
            }
        }

        // Set a value in an input field (works for text, password, etc.)
        public async Task SetValueAsync(string selector, string value)
        {
            _logger.Info($"Setting value '{value}' in input field with selector '{selector}'.");
            await WaitForElementAsync(selector);
            await _page.FillAsync(selector, value);
            _logger.Info($"Successfully set value in input field with selector '{selector}'.");
        }

        // Check if an element is visible
        public async Task<bool> IsElementVisibleAsync(string selector)
        {
            var isVisible = await _page.Locator(selector).IsVisibleAsync();
            return isVisible;
        }

        // Get text from an element
        public async Task<string> GetTextAsync(string selector)
        {
            var element = _page.Locator(selector);
            return await element.TextContentAsync();
        }


        // Get value from a field or element
        public async Task<string> GetValueAsync(string selector)
        {
            _logger.Info($"Getting value from field with selector '{selector}'.");
            await WaitForElementAsync(selector);
            string value = await _page.InputValueAsync(selector);
            _logger.Info($"Retrieved value: '{value}' from field with selector '{selector}'.");
            return value;
        }

        // Click an element after waiting for it to be clickable
        public async Task ClickElementAsync(string selector)
        {
            _logger.Info($"Clicking element with selector '{selector}'.");
            await WaitForElementAsync(selector);
            await _page.Locator(selector).ClickAsync();
            _logger.Info($"Successfully clicked element with selector '{selector}'.");
        }

        #endregion

        #region Frame Handling

        // Switch to a frame by name or ID
        public async Task<IFrame> SwitchToFrameAsync(string frameNameOrId)
        {
            _logger.Info($"Switching to frame with name or ID: {frameNameOrId}");
            var frame = _page.Frames.FirstOrDefault(f => f.Name == frameNameOrId || f.Url.Contains(frameNameOrId));
            if (frame == null)
            {
                _logger.Error($"Frame with name or ID '{frameNameOrId}' not found.");
                throw new Exception($"Frame with name or ID '{frameNameOrId}' not found.");
            }
            _logger.Info($"Successfully switched to frame with name or ID '{frameNameOrId}'.");
            return frame;
        }

        // Switch to a nested iframe
        public async Task<IFrame> SwitchToNestedFrameAsync(string outerFrameNameOrId, string innerFrameNameOrId)
        {
            _logger.Info($"Switching to nested frame with outer frame '{outerFrameNameOrId}' and inner frame '{innerFrameNameOrId}'.");
            var outerFrame = await SwitchToFrameAsync(outerFrameNameOrId);
            var innerFrame = outerFrame.ChildFrames.FirstOrDefault(f => f.Name == innerFrameNameOrId);
            if (innerFrame == null)
            {
                _logger.Error($"Inner frame with name or ID '{innerFrameNameOrId}' not found.");
                throw new Exception($"Inner frame with name or ID '{innerFrameNameOrId}' not found.");
            }
            _logger.Info($"Successfully switched to nested frame with inner frame '{innerFrameNameOrId}'.");
            return innerFrame;
        }

        // Find if a locator exists inside a frame and return true/false
        public bool IsLocatorFoundInsideFrameAsync(string frameNameOrId, string locatorSelector)
        {
            try
            {
                // Step 1: Switch to the correct frame
                var frame = SwitchToFrameAsync(frameNameOrId).Result;

                // Step 2: Locate the element inside the frame
                var element = frame.Locator(locatorSelector);

                // Step 3: Check if the element is visible
                bool isVisible = element.IsVisibleAsync().Result;

                // Return true if the element is visible, else false
                return isVisible;
            }
            catch (Exception ex)
            {
                // Log or handle the error based on your requirement
                // If any exception is caught, the locator wasn't found or something went wrong.
                return false;
            }
        }

        #endregion

        #region Drag-and-Drop

        // Perform a drag-and-drop operation
        public async Task DragAndDropInsideFrameAsync(string frameNameOrId, string sourceSelector, string targetSelector)
        {
            try
            {
                var frame = await SwitchToFrameAsync(frameNameOrId);

                var source = frame.Locator(sourceSelector);
                var target = frame.Locator(targetSelector);

                // Retry logic for drag-and-drop
                const int maxRetries = 3;
                for (int attempt = 0; attempt < maxRetries; attempt++)
                {
                    try
                    {
                        await source.HoverAsync();
                        await target.HoverAsync();
                        await source.DragToAsync(target);

                        // If drag-and-drop succeeds, break the retry loop
                        break;
                    }
                    catch (PlaywrightException ex)
                    {
                        if (attempt == maxRetries - 1) // On last attempt, rethrow the exception
                        {
                            throw new InvalidOperationException("Drag and Drop failed after multiple attempts.", ex);
                        }

                        // Log retry and wait before retrying
                        _logger.Warning($"Drag and Drop attempt {attempt + 1} failed: {ex.Message}. Retrying...");
                        await Task.Delay(1000); // Delay before retrying
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to perform drag and drop inside frame", ex);
                throw;
            }
        }

        #endregion

        #region Tab Handling

        // Switch to a tab by index
        public void SwitchToTab(int tabIndex)
        {
            _logger.Info($"Switching to tab index {tabIndex}.");
            var allWindows = _page.Context.Pages.ToList();
            if (allWindows.Count > tabIndex)
            {
                _page = allWindows[tabIndex];
                _logger.Info($"Successfully switched to tab index {tabIndex}.");
            }
            else
            {
                _logger.Error($"Tab index {tabIndex} does not exist.");
            }
        }

        // Switch to the last tab
        public void SwitchToLastTab()
        {
            _logger.Info("Switching to the last tab.");
            var allWindows = _page.Context.Pages.ToList();
            if (allWindows.Count > 0)
            {
                _page = allWindows.Last();
                _logger.Info("Successfully switched to the last tab.");
            }
            else
            {
                _logger.Error("No tabs available.");
            }
        }

        #endregion

        #region Waiting for API Calls

        // Wait for API requests to complete (useful for waiting for network traffic like API calls)
        public async Task WaitForAPIResponseAsync(string urlPattern, int timeoutInMillis = 20000)
        {
            _logger.Info($"Waiting for API response matching '{urlPattern}' within {timeoutInMillis} ms.");
            var responseTask = _page.WaitForResponseAsync(response => response.Url.Contains(urlPattern));
            var result = await Task.WhenAny(responseTask, Task.Delay(timeoutInMillis));

            if (result != responseTask)
            {
                _logger.Error($"API response for '{urlPattern}' not received within {timeoutInMillis} ms.");
                throw new TimeoutException($"API response for '{urlPattern}' not received within {timeoutInMillis} ms.");
            }

            var response = await responseTask;
            if (response.Status != 200)
            {
                _logger.Error($"API call failed with status {response.Status} for '{urlPattern}'.");
                throw new Exception($"API call failed with status {response.Status}.");
            }

            _logger.Info($"Received API response for '{urlPattern}' with status {response.Status}.");
        }

        #endregion

        #region Retry Logic

        // Retry a failed action multiple times
        public async Task RetryActionAsync(Func<Task> action, int maxRetries = MaxRetries, int delayInMillis = RetryDelayInMillis)
        {
            int attempts = 0;
            while (attempts < maxRetries)
            {
                try
                {
                    _logger.Info($"Attempt {attempts + 1} of {maxRetries}.");
                    await action();
                    return; // Exit on success
                }
                catch (Exception ex)
                {
                    attempts++;
                    if (attempts == maxRetries)
                    {
                        _logger.Error($"Action failed after {maxRetries} attempts. Error: {ex.Message}");
                        throw new Exception($"Action failed after {maxRetries} attempts. Error: {ex.Message}", ex);
                    }
                    _logger.Warning($"Action failed. Retrying in {delayInMillis} ms...");
                    await Task.Delay(delayInMillis); // Wait before retrying
                }
            }
        }

        #endregion

        #region Miscellaneous Utility Methods

        // Take a full-page screenshot
        public async Task TakeFullPageScreenshotAsync(string screenshotPath)
        {
            _logger.Info($"Taking full-page screenshot and saving to {screenshotPath}.");
            await _page.ScreenshotAsync(new PageScreenshotOptions
            {
                Path = screenshotPath,
                FullPage = true
            });
            _logger.Info($"Screenshot saved to {screenshotPath}.");
        }

        // Extract text from elements matching a regular expression
        public async Task<List<string>> ExtractTextByRegexAsync(string regexPattern)
        {
            _logger.Info($"Extracting text from elements matching pattern: {regexPattern}");
            var elements = await _page.Locator($"text={regexPattern}").AllTextContentsAsync();
            var matchedTexts = elements.Where(x => Regex.IsMatch(x, regexPattern)).ToList();
            _logger.Info($"Found {matchedTexts.Count} elements matching the pattern.");
            return matchedTexts;
        }

        #endregion
    }
}
