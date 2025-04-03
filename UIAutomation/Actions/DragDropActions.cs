using Microsoft.Playwright;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UIAutomation.Helpers;
using UIAutomation.Pages;
using static System.Net.Mime.MediaTypeNames;

namespace UIAutomation.Actions
{
    public class DragDropActions
    {
        private readonly DragDropPage _dragDropPage;

        public DragDropActions(IPage page)
        {
            _dragDropPage = new DragDropPage(page);
        }

        // Check if a popup exists
        public bool IsPopUpExists() => _dragDropPage.IsPopUpDisplayed();

        public async void DragAndDrop()
        {
            var frameId = "examples-iframe";
            var source = "div[data-recordid='1']";
            var target = "//table[@data-recordid='6']//div[text()='Drop patients here']";
            await _dragDropPage.DragAndDropInsideFrameAsync(frameId, source, target);
        }
    }
}
