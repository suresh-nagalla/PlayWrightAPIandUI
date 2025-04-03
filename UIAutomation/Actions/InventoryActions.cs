using Microsoft.Playwright;
using System;
using System.Linq;
using System.Threading.Tasks;
using UIAutomation.Helpers;
using UIAutomation.Pages;

namespace UIAutomation.Actions
{
    public class InventoryActions
    {
        private readonly IPage _page;
        private readonly InventoryPage _inventoryPage;
        private readonly CartPage _cartPage;
        private readonly Logger _logger = Logger.Instance;

        public InventoryActions(IPage page)
        {
            _page = page;
            _inventoryPage = new InventoryPage(page);
            _cartPage = new CartPage(page);
        }

        // Add item to cart and verify it's in the cart
        public async Task<bool> AddItemToCartAndVerify(string itemName)
        {
            try
            {
                _logger.Info($"Adding item to cart and verifying: {itemName}");

                // Add the item to the cart
                await _inventoryPage.AddItemToCart(itemName);

                // Verify cart count increased
                int cartCount = await _inventoryPage.GetCartItemCount();
                _logger.Info($"Current cart count: {cartCount}");

                // Navigate to cart
                await _inventoryPage.ClickCartIcon();

                // Verify cart page loaded
                bool cartLoaded = await _cartPage.IsLoaded();
                if (!cartLoaded)
                {
                    _logger.Warning("Cart page did not load properly");
                    return false;
                }

                // Verify item is in cart
                bool itemInCart = await _cartPage.IsItemInCart(itemName);
                if (itemInCart)
                {
                    _logger.Info($"Item successfully added to cart: {itemName}");
                }
                else
                {
                    _logger.Warning($"Item not found in cart: {itemName}");
                }

                return itemInCart;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error adding item to cart: {itemName}", ex);
                throw;
            }
        }

        // Remove item from cart and verify it's removed
        public async Task<bool> RemoveItemFromCartAndVerify(string itemName)
        {
            try
            {
                _logger.Info($"Removing item from cart and verifying: {itemName}");

                // Ensure we're on the cart page
                await _inventoryPage.ClickCartIcon();

                // Verify cart page loaded
                bool cartLoaded = await _cartPage.IsLoaded();
                if (!cartLoaded)
                {
                    _logger.Warning("Cart page did not load properly");
                    return false;
                }

                // Check if item is in cart first
                bool itemInCart = await _cartPage.IsItemInCart(itemName);
                if (!itemInCart)
                {
                    _logger.Warning($"Item not found in cart: {itemName}");
                    return false;
                }

                // Get cart count before removal
                int countBefore = await _cartPage.GetCartItemCount();
                _logger.Info($"Cart count before removal: {countBefore}");

                // Remove the item
                await _cartPage.RemoveItemFromCart(itemName);

                // Get cart count after removal
                int countAfter = await _cartPage.GetCartItemCount();
                _logger.Info($"Cart count after removal: {countAfter}");

                // Verify item is removed from cart
                bool itemRemoved = !await _cartPage.IsItemInCart(itemName);
                if (itemRemoved)
                {
                    _logger.Info($"Item successfully removed from cart: {itemName}");
                }
                else
                {
                    _logger.Warning($"Item still found in cart after removal: {itemName}");
                }

                return itemRemoved && (countAfter == countBefore - 1);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error removing item from cart: {itemName}", ex);
                throw;
            }
        }

        // Get the first available item name from inventory
        public async Task<string> GetFirstAvailableItemName()
        {
            try
            {
                _logger.Info("Getting first available item name");

                var itemNames = await _inventoryPage.GetInventoryItemNames();

                if (itemNames.Any())
                {
                    _logger.Info($"First available item: {itemNames[0]}");
                    return itemNames[0];
                }

                _logger.Warning("No items found in inventory");
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.Error("Error getting first available item", ex);
                throw;
            }
        }

        // Get the current cart item count
        public async Task<int> GetCartItemCount() => await _inventoryPage.GetCartItemCount();

        // Check if the page is loaded
        public async Task<bool> IsPageLoaded() => await _inventoryPage.IsLoaded();
    }
}
