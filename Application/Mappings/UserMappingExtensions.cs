using Application.DTO.User;
using Application.Helpers;
using Domain.Entities;

namespace Application.Mappings
{
    public static class UserMappingExtensions
    {
        /// <summary>   
        /// Maps a UserRequest to a User entity.
        public static User ToEntity(this UserRequest request)
        {
            return new User
            {
                UserId = request.UserId.ToUpper(), 
                Name = request.Name.ToUpper(),         
                Email = request.Email.ToLower(),   
                IsActive = request.IsActive,   
                IsAdmin = request.IsAdmin,
                IsTechAccount = request.IsTechAccount
            };
        }

        /// <summary>
        /// maps a User entity to a UserResponse.
        public static UserResponse ToResponse(this User entity)
        {
            return new UserResponse
            {
                UserId = entity.UserId,
                Name = entity.Name,
                Email = entity.Email,
                PasswordHash = entity.PasswordHash, // Assuming PasswordHash is the password to be returned
                IsActive = entity.IsActive,
                IsTechAccount = entity.IsTechAccount,
                CreatedAt = DateTimeHelper.ConvertUtcToTimeZone(entity.CreatedAt, "America/Sao_Paulo"),
                UpdatedAt = entity.UpdatedAt.HasValue ? 
                                DateTimeHelper.ConvertUtcToTimeZone(entity.UpdatedAt.Value, "America/Sao_Paulo") : (DateTime?)null,
                IsAdmin = entity.IsAdmin
            };
        }
    }
}
