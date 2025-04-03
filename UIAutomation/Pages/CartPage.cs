namespace UIAutomation.Pages
{
    using Microsoft.Playwright;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UIAutomation.Helpers;

    public class CartPage
    {
        private readonly IPage _page;
        private readonly Logger _logger = Logger.Instance;

        // Locators
        private ILocator CartItems => _page.Locator(".cart_item");
        private ILocator CheckoutButton => _page.Locator("[data-test=\"checkout\"]");
        private ILocator ContinueShoppingButton => _page.Locator("[data-test=\"continue-shopping\"]");
        private string CartItemNameSelector = ".inventory_item_name";
        private string RemoveButtonSelector = "[data-test^=\"remove\"]";

        public CartPage(IPage page)
        {
            _page = page;
        }

        // Check if the Cart page is loaded
        public async Task<bool> IsLoaded()
        {
            _logger.Info("Checking if Cart page is loaded");
            return await _page.Locator(".cart_list").IsVisibleAsync();
        }

        // Get the count of items in the cart
        public async Task<int> GetCartItemCount()
        {
            _logger.Info("Getting cart item count");
            return await CartItems.CountAsync();
        }

        // Get all item names in the cart
        public async Task<List<string>> GetCartItemNames()
        {
            _logger.Info("Getting all cart item names");

            var itemNames = new List<string>();
            var itemNameElements = _page.Locator(CartItemNameSelector);
            var count = await itemNameElements.CountAsync();

            for (int i = 0; i < count; i++)
            {
                var nameText = await itemNameElements.Nth(i).TextContentAsync();
                if (!string.IsNullOrEmpty(nameText))
                {
                    itemNames.Add(nameText);
                }
            }

            return itemNames;
        }

        // Remove a specific item from the cart by its name
        public async Task RemoveItemFromCart(string itemName)
        {
            _logger.Info($"Removing item from cart: {itemName}");

            // Find the item by name
            var itemLocator = _page.Locator(CartItemNameSelector, new PageLocatorOptions { HasText = itemName });

            // Find the parent container of the item
            var itemContainer = itemLocator.Locator("xpath=ancestor::div[contains(@class, 'cart_item')]");

            // Locate the Remove button within the item container and click it
            var removeButton = itemContainer.Locator(RemoveButtonSelector);
            await removeButton.ClickAsync();

            _logger.Info($"Item removed from cart: {itemName}");
        }

        // Click the checkout button
        public async Task ClickCheckoutButton()
        {
            _logger.Info("Clicking checkout button");
            await CheckoutButton.ClickAsync();
        }

        // Click the continue shopping button
        public async Task ClickContinueShoppingButton()
        {
            _logger.Info("Clicking continue shopping button");
            await ContinueShoppingButton.ClickAsync();
        }

        // Check if a specific item is in the cart
        public async Task<bool> IsItemInCart(string itemName)
        {
            _logger.Info($"Checking if item is in cart: {itemName}");

            var items = await GetCartItemNames();
            return items.Contains(itemName);
        }
    }
}
