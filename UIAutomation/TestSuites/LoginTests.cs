namespace UIAutomation.TestSuites
{
    using Microsoft.Playwright;
    using NUnit.Framework;
    using System.Threading.Tasks;
    using UIAutomation.Drivers;
    using UIAutomation.Helpers;
    using static System.Net.Mime.MediaTypeNames;
    using UIAutomation.Pages;
    using UIAutomation.Actions;

    [TestFixture]
    [Parallelizable(ParallelScope.Self)]
    public class LoginTests : TestBase
    {
        private LoginActions _loginAction;

        [SetUp]
        public override async Task Setup()
        {
            await base.Setup();
            _loginAction = new LoginActions(_page);
        }

        [Test]
        [Description("Verify that valid login leads to inventory page")]
        public async Task ValidLogin_ShouldNavigateToInventory()
        {
            bool success = await _loginAction.PerformValidLogin();
            Assert.That(success, Is.True, "Valid login should navigate to inventory page");
        }

        [Test]
        [Description("Verify that invalid login shows error message")]
        public async Task InvalidLogin_ShouldShowError()
        {
            string errorMessage = await _loginAction.PerformInvalidLogin("invalid_user", "invalid_password");
            Assert.That(errorMessage, Is.Not.Empty, "Invalid login should show an error message");
        }

        [Test]
        [Description("Verify that empty login credentials show error")]
        public async Task EmptyCredentials_ShouldShowError()
        {
            bool hasError = await _loginAction.PerformLoginWithEmptyCredentials();
            Assert.That(hasError, Is.True, "Empty credentials should show an error message");
        }
    }
}
