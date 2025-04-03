namespace UIAutomation.Pages
{
    using Microsoft.Playwright;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UIAutomation.Helpers;

    public class InventoryPage : PlaywrightPageBase
    {
        private readonly Logger _logger = Logger.Instance;
        private readonly IPage _page;
        public InventoryPage(IPage page) : base(page) { _page = page; }

        // Locators
        private ILocator InventoryItems => _page.Locator(".inventory_item");
        private ILocator ItemName => _page.Locator(".inventory_item_name");
        private ILocator ShoppingCartBadge => _page.Locator(".shopping_cart_badge");
        private ILocator ShoppingCartLink => _page.Locator(".shopping_cart_link");
        private string AddToCartButtonSelector = "[data-test^=\"add-to-cart\"]";
        private string RemoveButtonSelector = "[data-test^=\"remove\"]";
        private string ItemNameSelector = ".inventory_item_name";

        public async Task<int> GetItemCount() => await InventoryItems.CountAsync();

        public async Task AddItemToCart(string itemName)
        {
            var itemLocator = _page.Locator(ItemNameSelector, new PageLocatorOptions { HasText = itemName });
            var itemContainer = itemLocator.Locator("xpath=ancestor::div[contains(@class, 'inventory_item')]");
            var addButton = itemContainer.Locator(AddToCartButtonSelector);
            await addButton.ClickAsync();
        }

        public async Task RemoveItemFromCart(string itemName)
        {
            var itemLocator = _page.Locator(ItemNameSelector, new PageLocatorOptions { HasText = itemName });
            var itemContainer = itemLocator.Locator("xpath=ancestor::div[contains(@class, 'inventory_item')]");
            var removeButton = itemContainer.Locator(RemoveButtonSelector);
            await removeButton.ClickAsync();
        }

        public async Task<int> GetCartItemCount()
        {
            if (await ShoppingCartBadge.IsVisibleAsync())
            {
                string badgeText = await ShoppingCartBadge.TextContentAsync() ?? "0";
                return int.TryParse(badgeText, out int count) ? count : 0;
            }
            return 0;
        }

        public async Task<bool> IsLoaded()
        {
            _logger.Info("Checking if Inventory page is loaded");
            return await _page.Locator("div.inventory_item").First.IsVisibleAsync();
        }

        public async Task ClickCartIcon() => await ShoppingCartLink.ClickAsync();

        public async Task<string[]> GetInventoryItemNames()
        {
            var itemCount = await InventoryItems.CountAsync();
            string[] itemNames = new string[itemCount];

            for (int i = 0; i < itemCount; i++)
            {
                var itemName = await ItemName.Nth(i).TextContentAsync();
                itemNames[i] = itemName;
            }

            return itemNames;
        }
    }
}
