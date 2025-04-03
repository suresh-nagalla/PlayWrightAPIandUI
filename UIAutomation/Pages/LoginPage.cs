using Microsoft.Playwright;
using System.Threading.Tasks;
namespace UIAutomation.Pages
{


    public class LoginPage : PlaywrightPageBase
    {
        public LoginPage(IPage page) : base(page) { }

        // Locators
        private ILocator UsernameInput => _page.Locator("[data-test=\"username\"]");
        private ILocator PasswordInput => _page.Locator("[data-test=\"password\"]");
        private ILocator LoginButton => _page.Locator("[data-test=\"login-button\"]");
        private ILocator ErrorMessage => _page.Locator("[data-test=\"error\"]");

        public async Task EnterUsername(string username) => await SetValueAsync("[data-test=\"username\"]", username);
        public async Task EnterPassword(string password) => await SetValueAsync("[data-test=\"password\"]", password);
        public async Task ClickLoginButton() => await ClickElementAsync("[data-test=\"login-button\"]");

        public async Task<bool> IsErrorMessageDisplayed() => await IsElementVisibleAsync("[data-test=\"error\"]");

        public async Task<string> GetErrorMessage() => await GetTextAsync("[data-test=\"error\"]");

        public async Task NavigateToAsync(string url) => await NavigateToPageAsync(url);

        public async Task Login(string username, string password)
        {
            await EnterUsername(username);
            await EnterPassword(password);
            await ClickLoginButton();
        }
    }
}
