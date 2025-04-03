namespace UIAutomation.Actions
{
    using Microsoft.Playwright;
    using System;
    using System.Threading.Tasks;
    using UIAutomation.Helpers;
    using UIAutomation.Pages;

    public class LoginActions
    {
        private readonly IPage _page;
        private readonly LoginPage _loginPage;
        private readonly InventoryActions _inventoryAction;
        private readonly Logger _logger = Logger.Instance;
        private readonly UIConfigManager _config = UIConfigManager.Instance;

        public LoginActions(IPage page)
        {
            _page = page;
            _loginPage = new LoginPage(page);
            _inventoryAction = new InventoryActions(page);
        }

        // Perform valid login
        public async Task<bool> PerformValidLogin(string userType = "StandardUser")
        {
            try
            {
                _logger.Info($"Performing valid login with {userType}");

                await _loginPage.NavigateToPageAsync(_config.GetBaseUrl());
                await _loginPage.Login(_config.GetUsername(userType), _config.GetPassword(userType));

                bool success = await _inventoryAction.IsPageLoaded();

                if (success)
                {
                    _logger.Info("Login successful, Inventory page loaded");
                }
                else
                {
                    _logger.Warning("Login failed, Inventory page not loaded");
                }

                return success;
            }
            catch (Exception ex)
            {
                _logger.Error("Error during login process", ex);
                throw;
            }
        }

        // Perform invalid login
        public async Task<string> PerformInvalidLogin(string username, string password)
        {
            try
            {
                _logger.Info("Performing invalid login");

                await _loginPage.NavigateToAsync(_config.GetBaseUrl());
                await _loginPage.Login(username, password);

                if (await _loginPage.IsErrorMessageDisplayed())
                {
                    string errorMessage = await _loginPage.GetErrorMessage();
                    _logger.Info($"Invalid login error message: {errorMessage}");
                    return errorMessage;
                }

                _logger.Warning("No error message displayed for invalid login");
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.Error("Error during invalid login process", ex);
                throw;
            }
        }

        // Perform login with empty credentials
        public async Task<bool> PerformLoginWithEmptyCredentials()
        {
            try
            {
                _logger.Info("Performing login with empty credentials");

                await _loginPage.NavigateToAsync(_config.GetBaseUrl());
                await _loginPage.Login("", "");

                bool hasError = await _loginPage.IsErrorMessageDisplayed();

                if (hasError)
                {
                    string errorMessage = await _loginPage.GetErrorMessage();
                    _logger.Info($"Empty credentials error message: {errorMessage}");
                }
                else
                {
                    _logger.Warning("No error message displayed for empty credentials");
                }

                return hasError;
            }
            catch (Exception ex)
            {
                _logger.Error("Error during empty credentials login process", ex);
                throw;
            }
        }
    }
}
