namespace APIAutomation.TestData
{
    using APIAutomation.Models;

    public static class ReqResFactory
    {
        public static CreateUserRequest CreateUser(string name, string job) =>
            new CreateUserRequest { name = name, job = job };

        public static RegisterRequest CreateRegistration(string email, string password) =>
            new RegisterRequest { Email = email, Password = password };
    }
}
