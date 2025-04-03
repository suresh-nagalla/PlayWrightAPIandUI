namespace APIAutomation.TestSuites
{
    using APIAutomation.Clients;
    using APIAutomation.Helpers;
    using APIAutomation.Models;
    using APIAutomation.TestData;
    using NUnit.Framework;
    using System.Threading.Tasks;

    [TestFixture]
    [Parallelizable(ParallelScope.Self)]
    public class UserTests
    {
        private PlaywrightFixture? _playwrightFixture;
        private ReqResClient? _client;
        private readonly Logger _logger = Logger.Instance;

        [SetUp]
        public void Setup()
        {
            _logger.Info("Setting up PlaywrightFixture and ReqResClient");
            _playwrightFixture = new PlaywrightFixture();
            _client = new ReqResClient(_playwrightFixture.RequestContext);
        }

        [TearDown]
        public void TearDown()
        {
            _logger.Info("Disposing PlaywrightFixture");
            _playwrightFixture?.Dispose();
        }

        [Test]
        public async Task CreateUser_ShouldReturnValidId()
        {
            var user = ReqResFactory.CreateUser("morpheus", "leader");
            var result = await _client!.CreateUserAsync(user);
            Assert.That(result?.Id, Is.Not.Null);
        }

        [Test]
        public async Task GetUserById_ShouldReturnUserData()
        {
            var result = await _client!.GetUserByIdAsync(2);
            Assert.That(result?.Data?.Id, Is.EqualTo(2));
        }

        [Test]
        public async Task ListUsers_ShouldReturnMultipleEntries()
        {
            var result = await _client!.ListUsersAsync();
            Assert.That(result?.Data?.Count, Is.GreaterThan(0));
        }

        [Order(100)]
        [Test]
        public async Task DeleteUser_ShouldSucceed()
        {
            var response = await _client!.DeleteUserAsync(2);
            Assert.That(response, Is.True);
        }

        [Test]
        public async Task GetUpdateDeleteUser_ShouldSucceed()
        {
            // Fetch
            var result = await _client!.GetUserByIdAsync(2);
            Assert.That(result?.Data?.Id, Is.EqualTo(2));

            // Update
            var updated = await _client.UpdateUserAsync("2", new { name = "neo", job = "the architect" });
            Assert.That(updated?.Job, Is.EqualTo("the architect"));
            _logger.Info($"Updated user job to: {updated?.Job}");

            // Delete
            var deleted = await _client.DeleteUserAsync(2);
            Assert.That(deleted, Is.True);
            _logger.Info($"Deleted user with ID: {result?.Data?.First_Name}");
        }
    }
}
