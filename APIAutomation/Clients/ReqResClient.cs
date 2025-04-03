namespace APIAutomation.Clients
{
    using APIAutomation.Models;
    using Microsoft.Playwright;
    using System.Threading.Tasks;

    public class ReqResClient : BaseClient
    {
        private const string USERS_ENDPOINT = "users";
        private const string REGISTER_ENDPOINT = "/register";

        public ReqResClient(IAPIRequestContext requestContext) : base(requestContext)
        {
        }

        public async Task<CreateUserResponse?> CreateUserAsync(CreateUserRequest user) =>
            await ExecuteWithRetryAsync<CreateUserResponse>(
               USERS_ENDPOINT, "POST", user);
        //await ExecuteWithRetryAsync<CreateUserResponse>("/api/users", "POST", new { name = "morpheus", job = "leader" });

        public async Task<UserResponse?> GetUserByIdAsync(int id) =>
            await ExecuteWithRetryAsync<UserResponse>(
                $"{USERS_ENDPOINT}/{id}", "GET", treat404AsNull: true);

        public async Task<UserListResponse?> ListUsersAsync() =>
            await ExecuteWithRetryAsync<UserListResponse>(
                USERS_ENDPOINT, "GET");

        public async Task<bool> DeleteUserAsync(int id) =>
            await ExecuteWithRetryAsync(
                $"{USERS_ENDPOINT}/{id}", "DELETE");

        public async Task<RegisterResponse?> RegisterUserAsync(RegisterRequest request) =>
            await ExecuteWithRetryAsync<RegisterResponse>(
                REGISTER_ENDPOINT, "POST", request);

        public async Task<UpdateUserResponse?> UpdateUserAsync(string id, object updatedFields) =>
            await ExecuteWithRetryAsync<UpdateUserResponse>(
                $"{USERS_ENDPOINT}/{id}", "PUT", updatedFields);
    }
}
