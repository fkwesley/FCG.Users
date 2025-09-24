using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UsersDbContext _context;

        public IEnumerable<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }

        public User? GetUserById(string userId)
        {
            return _context.Users.FirstOrDefault(g => g.UserId == userId);
        }

        public UserRepository(UsersDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public User AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
        }

        public User UpdateUser(User user)
        {
            // Procura por uma instância já rastreada desse usuário
            var trackedEntity = _context.ChangeTracker.Entries<User>().FirstOrDefault(e => e.Entity.UserId == user.UserId);

            // Desanexa a entidade rastreada para evitar conflito
            if (trackedEntity != null)
                trackedEntity.State = EntityState.Detached;

            _context.Users.Update(user);
            _context.SaveChanges();
            return user;
        }

        public bool DeleteUser(string id)
        {
            var user = GetUserById(id);

            if (user != null)
            {
                user.IsActive = false;
                _context.Users.Update(user);
                _context.SaveChanges();
                return true;
            }
            else
                throw new KeyNotFoundException($"User with ID {id} not found.");
        }


    }
}
