namespace UIAutomation.TestSuites
{
    using Microsoft.Playwright;
    using NUnit.Framework;
    using System.Threading.Tasks;
    using static System.Net.Mime.MediaTypeNames;
    using UIAutomation.Drivers;
    using UIAutomation.Pages;
    using UIAutomation.Actions;

    [TestFixture]
    [Parallelizable(ParallelScope.Self)]
    public class InventoryTests : TestBase
    {
        private InventoryActions _inventoryAction;

        [SetUp]
        public override async Task Setup()
        {
            await base.Setup();
            _inventoryAction = new InventoryActions(_page);
        }

        [Test]
        [Description("Verify that item can be added to cart")]
        public async Task AddToCart_ShouldAddItemToCart()
        {
            string itemName = await _inventoryAction.GetFirstAvailableItemName();
            Assert.That(itemName, Is.Not.Empty, "No items available in inventory");

            var result = await _inventoryAction.AddItemToCartAndVerify(itemName);
            Assert.That(result, Is.True, "Item should be added to the cart");
        }

        [Test]
        [Description("Verify that item can be removed from cart")]
        public async Task RemoveFromCart_ShouldRemoveItemFromCart()
        {
            string itemName = await _inventoryAction.GetFirstAvailableItemName();
            Assert.That(itemName, Is.Not.Empty, "No items available in inventory");

            await _inventoryAction.AddItemToCartAndVerify(itemName);
            int cartCountBefore = await _inventoryAction.GetCartItemCount();

            await _inventoryAction.RemoveItemFromCartAndVerify(itemName);
            int cartCountAfter = await _inventoryAction.GetCartItemCount();

            Assert.That(cartCountAfter, Is.LessThan(cartCountBefore), "Item should be removed from the cart");
        }
    }
}
