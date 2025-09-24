using Application.DTO.User;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IUserService
    {
        IEnumerable<UserResponse> GetAllUsers();
        UserResponse GetUserById(string userId);
        UserResponse AddUser(UserRequest user);
        UserResponse UpdateUser(UserRequest user);
        bool DeleteUser(string userId);
        User? ValidateCredentials(string userId, string password);
    }
}