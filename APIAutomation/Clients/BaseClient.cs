using APIAutomation.Helpers;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace APIAutomation.Clients
{
    public abstract class BaseClient
    {
        protected readonly Logger _logger = Logger.Instance;
        protected readonly APIConfigManager _config = APIConfigManager.Instance;
        protected readonly IAPIRequestContext _requestContext;

        protected BaseClient(IAPIRequestContext requestContext)
        {
            _requestContext = requestContext;
        }

        protected async Task<T?> ExecuteWithRetryAsync<T>(
     string endpoint,
     string method,
     object? data = null,
     bool treat404AsNull = false) where T : class
        {
            int maxRetries = _config.GetRetryCount();
            int retryDelay = _config.GetRetryDelayMilliseconds();
            Exception? lastException = null;
            string? lastRequestDetails = null;
            string? lastResponseDetails = null;
            int? lastStatusCode = null;

            for (int attempt = 0; attempt <= maxRetries; attempt++)
            {
                try
                {
                    if (attempt > 0)
                    {
                        _logger.Info($"Retry attempt {attempt} of {maxRetries}");
                        await Task.Delay(retryDelay * attempt);
                    }

                    // Prepare request options based on the HTTP method
                    var options = new APIRequestContextOptions
                    {
                        Method = method,
                        Headers = new Dictionary<string, string>
                        {
                            { "Content-Type", "application/json" }
                        }
                    };

                    // Add data/body if provided
                    string requestBody = "null";
                    if (data != null)
                    {
                        requestBody = JsonSerializer.Serialize(data);
                        options.DataString = requestBody;
                    }

                    // Store request details
                    lastRequestDetails = $"URL: {_config.GetBaseUrl()}{endpoint}, Method: {method}, Body: {requestBody}";
                    _logger.Info($"Sending {method} request to {endpoint}");

                    var response = await _requestContext.FetchAsync(endpoint, options);
                    lastStatusCode = response.Status;

                    string responseBody = await response.TextAsync();
                    lastResponseDetails = $"Status: {response.Status}, Headers: {JsonSerializer.Serialize(response.Headers)}, Body: {responseBody}";
                    _logger.Info($"Response status: {response.Status}, body: {responseBody}");

                    if (response.Ok)
                    {
                        if (string.IsNullOrEmpty(responseBody))
                            return null;

                        return JsonSerializer.Deserialize<T>(responseBody, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                    }

                    if (treat404AsNull && response.Status == (int)HttpStatusCode.NotFound)
                    {
                        _logger.Info("Received 404 - returning null as configured.");
                        return null;
                    }

                    _logger.Warning($"Request failed with status code {response.Status}: {responseBody}");
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    _logger.Error("Request threw an exception", ex);
                }
            }

            // Construct detailed error message including request and response details
            var errorMessage = new System.Text.StringBuilder();
            errorMessage.AppendLine($"Request failed after {maxRetries} retries.");
            errorMessage.AppendLine($"Last request: {lastRequestDetails}");
            errorMessage.AppendLine($"Last response: {lastResponseDetails ?? "No response received"}");
            errorMessage.AppendLine($"Last status code: {(lastStatusCode.HasValue ? lastStatusCode.ToString() : "N/A")}");

            throw new Exception(errorMessage.ToString(), lastException);
        }

        protected async Task<bool> ExecuteWithRetryAsync(
    string endpoint,
    string method,
    object? data = null)
        {
            int maxRetries = _config.GetRetryCount();
            int retryDelay = _config.GetRetryDelayMilliseconds();
            Exception? lastException = null;
            string? lastRequestDetails = null;
            string? lastResponseDetails = null;
            int? lastStatusCode = null;

            // Log the base URL to ensure it's correct
            _logger.Info($"Base URL: {_config.GetBaseUrl()}");

            for (int attempt = 0; attempt <= maxRetries; attempt++)
            {
                try
                {
                    if (attempt > 0)
                    {
                        _logger.Info($"Retry attempt {attempt} of {maxRetries}");
                        await Task.Delay(retryDelay * attempt);
                    }

                    // Prepare request options based on the HTTP method
                    var options = new APIRequestContextOptions
                    {
                        Method = method,
                        Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                }
                    };

                    // Add data/body if provided
                    string requestBody = "null";
                    if (data != null)
                    {
                        requestBody = JsonSerializer.Serialize(data);
                        options.DataString = requestBody;
                    }

                    // Store request details
                    lastRequestDetails = $"URL: {_config.GetBaseUrl()}{endpoint}, Method: {method}, Body: {requestBody}";
                    _logger.Info($"Sending {method} request to {_config.GetBaseUrl()}{endpoint} with body: {requestBody}");

                    // Send the request
                    var response = await _requestContext.FetchAsync(endpoint, options);
                    lastStatusCode = response.Status;

                    // Get response body
                    string responseBody = await response.TextAsync();
                    lastResponseDetails = $"Status: {response.Status}, Headers: {JsonSerializer.Serialize(response.Headers)}, Body: {responseBody}";

                    // Log response details
                    _logger.Info($"Response received: {lastResponseDetails}");

                    // Handle successful response or 404 (if configured)
                    if (response.Ok || response.Status == (int)HttpStatusCode.NotFound)
                    {
                        return true; // Successfully completed request
                    }

                    // Log failure for non-2xx status codes
                    _logger.Warning($"Request failed with status code {response.Status}: {responseBody}");
                }
                catch (Exception ex)
                {
                    // Log exception and continue retrying
                    lastException = ex;
                    _logger.Error($"Request threw an exception on attempt {attempt}: {ex.Message}", ex);
                }
            }

            // After retries, throw exception with detailed request/response information
            var errorMessage = new StringBuilder();
            errorMessage.AppendLine($"Request failed after {maxRetries} retries.");
            errorMessage.AppendLine($"Last request: {lastRequestDetails}");
            errorMessage.AppendLine($"Last response: {lastResponseDetails ?? "No response received"}");
            errorMessage.AppendLine($"Last status code: {(lastStatusCode.HasValue ? lastStatusCode.ToString() : "N/A")}");

            throw new Exception(errorMessage.ToString(), lastException);
        }

    }
}