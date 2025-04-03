using Microsoft.Playwright;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using UIAutomation.Actions;
using UIAutomation.Drivers;
using UIAutomation.Helpers;
using UIAutomation.Pages;

namespace UIAutomation.TestSuites
{
    [TestFixture]
    public class DragDropTests
    {
        private PlaywrightManager _playwrightManager;
        private IPage _page;
        private DragDropActions _dragDropActions;
        private readonly Logger _logger = Logger.Instance;
        private string Url = "https://examples.sencha.com/extjs/7.8.0/examples/classic/dd/dragdropzones.html";
        [SetUp]
        public async Task Setup()
        {
            _logger.Info("Setting up DragDropTests");

            _playwrightManager = new PlaywrightManager();
            await _playwrightManager.InitializeAsync();

            _page = _playwrightManager.GetPage();
            _dragDropActions = new DragDropActions(_page);

            _logger.Info("DragDropTests setup completed");
        }

        [TearDown]
        public async Task TearDown()
        {
            if (TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed)
            {
                await _playwrightManager.TakeScreenshotAsync($"DragDropTest_Failure_{TestContext.CurrentContext.Test.Name}");
            }

            await _playwrightManager.CloseAsync();
        }

        [Test]
        public async Task DragAndDrop_InsideIframe_Works()
        {
            await _page.GotoAsync(Url);
            await _dragDropActions.DragAndDrop();
            Assert.That(_dragDropActions.IsPopUpExists(), Is.True);
        }
    }
}
