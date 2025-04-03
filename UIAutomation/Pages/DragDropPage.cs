using Microsoft.Playwright;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UIAutomation.Pages
{
    public class DragDropPage : PlaywrightPageBase
    {
        private readonly IPage _page;

        // Locate the iframe with drag-and-drop functionality
        private IFrame _frame => _page.FrameByUrl(new Regex("dragdropzones.html"))!;

        public DragDropPage(IPage page) : base(page)
        {
            _page = page;
        }

        // Navigate to the Drag-and-Drop example page
        public async Task NavigateAsync() => await _page.GotoAsync("https://examples.sencha.com/extjs/7.8.0/examples/classic/dd/dragdropzones.html");

        // Check if the pop-up with the 'OK' button is displayed in the iframe
        public bool IsPopUpDisplayed()
        {
            return IsLocatorFoundInsideFrameAsync("examples-iframe", "//a//span[text()='OK']").Result;
        }
    }
}
