using Microsoft.Playwright;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using UIAutomation.Drivers;
using UIAutomation.Helpers;
using UIAutomation.Actions;

namespace UIAutomation.Drivers
{
    public class TestBase
    {
        protected PlaywrightManager? _playwrightManager;
        protected IPage? _page;
        protected LoginActions? _loginActions;
        protected readonly Logger _logger = Logger.Instance;

        [SetUp]
        public virtual async Task Setup()
        {
            try
            {
                _logger.Info($"Setting up {GetType().Name}");

                _playwrightManager = new PlaywrightManager();
                await _playwrightManager.InitializeAsync();

                _page = _playwrightManager.GetPage();
                _loginActions = new LoginActions(_page);

                if (_loginActions != null)
                {
                    bool loginSuccess = await _loginActions.PerformValidLogin();
                    if (!loginSuccess)
                    {
                        throw new Exception("Failed to login during test setup");
                    }
                }

                _logger.Info($"{GetType().Name} setup completed");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error in {GetType().Name} setup", ex);
                throw;
            }
        }

        [TearDown]
        public virtual async Task TearDown()
        {
            try
            {
                _logger.Info($"Tearing down {GetType().Name}");

                if (TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed)
                {
                    if (_playwrightManager != null)
                    {
                        string testName = TestContext.CurrentContext.Test.Name;
                        await _playwrightManager.TakeScreenshotAsync($"{GetType().Name}_Failure_{testName}");
                    }
                }

                if (_playwrightManager != null)
                {
                    await _playwrightManager.CloseAsync();
                }

                _logger.Info($"{GetType().Name} teardown completed");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error in {GetType().Name} teardown", ex);
                throw;
            }
        }
    }
}
