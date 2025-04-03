using System.Collections.Generic;

namespace APIAutomation.Models
{
    public class UserData
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Avatar { get; set; }
    }

    public class UserResponse
    {
        public UserData Data { get; set; }
    }

    public class UserListResponse
    {
        public List<UserData> Data { get; set; }
    }

    public class CreateUserRequest
    {
        public string name { get; set; }
        public string job { get; set; }
    }

    public class CreateUserResponse
    {
        public string Id { get; set; }
        public string CreatedAt { get; set; }
    }

    public class RegisterRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class RegisterResponse
    {
        public string Id { get; set; }
        public string Token { get; set; }
    }

    public class UpdateUserResponse
    {
        public string Name { get; set; }
        public string Job { get; set; }
        public string UpdatedAt { get; set; }
    }

}
