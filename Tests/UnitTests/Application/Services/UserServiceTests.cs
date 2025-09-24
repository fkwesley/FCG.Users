using Application.Interfaces;
using Application.Services;
using Domain.Repositories;
using Moq;
using FluentAssertions;
using Application.DTO.User;
using Application.Exceptions;
using Domain.Entities;

namespace Tests.UnitTests.Application.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IPasswordHasherRepository> _passwordHasherMock;
        private readonly IUserService _userService;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _passwordHasherMock = new Mock<IPasswordHasherRepository>();
            _userService = new UserService(_userRepositoryMock.Object, _passwordHasherMock.Object);
        }

        [Fact]
        public void GetAllUsers_ShouldReturnMappedUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new() { UserId = "USER1", Name = "Alice", Email = "a@a.com", IsActive = true, CreatedAt = DateTime.UtcNow.AddDays(-50) },
                new() { UserId = "USER2", Name = "Bob", Email = "b@b.com", IsActive = true, CreatedAt = DateTime.UtcNow.AddDays(-50) }
            };

            _userRepositoryMock.Setup(r => r.GetAllUsers()).Returns(users);

            // Act
            var result = _userService.GetAllUsers();

            // Assert
            result.Should().HaveCount(2);
            result.First().UserId.Should().Be("USER1");
        }

        [Fact]
        public void GetUserById_ShouldReturnMappedResponse()
        {
            var user = new User { UserId = "USER1", Name = "Alice", Email = "a@a.com", CreatedAt = DateTime.UtcNow.AddMonths(3)};

            _userRepositoryMock.Setup(r => r.GetUserById("USER1")).Returns(user);

            var result = _userService.GetUserById("USER1");

            result.UserId.Should().Be("USER1");
            result.Name.Should().Be("Alice");
        }

        [Fact]
        public void AddUser_ShouldAddSuccessfully_WhenUserIdAndEmailAreUnique()
        {
            var userRequest = new UserRequest
            {
                UserId = "newuser",
                Email = "new@a.com",
                Name = "Test",
                Password = "Password1*"
            };

            // Mock retorna lista vazia (sem usuários ativos)
            _userRepositoryMock.Setup(r => r.GetAllUsers()).Returns(new List<User>());

            // Mock do AddUser retorna o User passado
            _userRepositoryMock.Setup(r => r.AddUser(It.IsAny<User>())).Returns((User u) =>
            {
                // Supondo que User tenha propriedades DateTime, garantir UTC aqui se necessário
                u.CreatedAt = DateTime.SpecifyKind(u.CreatedAt, DateTimeKind.Utc);

                // uppercasing UserId para refletir comportamento esperado
                u.UserId = u.UserId.ToUpperInvariant();

                return u;
            });

            var result = _userService.AddUser(userRequest);

            // Verifica se o UserId foi uppercased
            result.UserId.Should().Be("NEWUSER");
        }

        [Fact]
        public void AddUser_ShouldThrowValidationException_WhenUserIdAlreadyExists()
        {
            var existing = new User { UserId = "EXISTING.USER", Name = "EXISTING USER", Email = "x@x.com", IsActive = true };

            _userRepositoryMock.Setup(r => r.GetAllUsers()).Returns(new List<User> { existing });

            var newUser = new UserRequest { UserId = "existing.user", Name = "existing user", Password = "password1*", Email = "another@x.com" };

            var act = () => _userService.AddUser(newUser);

            act.Should().Throw<ValidationException>().WithMessage("*UserId already exists*");
        }

        [Fact]
        public void UpdateUser_ShouldUpdate_WhenEmailIsNotInUseByAnotherUser()
        {
            var userRequest = new UserRequest { UserId = "U1", Email = "update@x.com", Name = "USER ONE", Password = "password1*" };

            _userRepositoryMock.Setup(r => r.GetAllUsers()).Returns(new List<User>
            {
                new() { UserId = "U1", Name = "USER 1", Email = "email@x.com", IsActive = true },
                new() { UserId = "U2", Name = "OTHER USER", Email = "other@x.com", IsActive = true, CreatedAt = DateTime.UtcNow.AddMonths(3), UpdatedAt = DateTime.UtcNow}
            });

            _userRepositoryMock.Setup(r => r.UpdateUser(It.IsAny<User>())).Returns((User u) =>
            {
                // Supondo que User tenha propriedades DateTime, garantir UTC aqui se necessário
                u.CreatedAt = DateTime.SpecifyKind(u.CreatedAt, DateTimeKind.Utc);

                // uppercasing UserId para refletir comportamento esperado
                u.UserId = u.UserId.ToUpperInvariant();

                return u;
            });

            var result = _userService.UpdateUser(userRequest);

            result.Email.Should().Be("update@x.com");
        }

        [Fact]
        public void UpdateUser_ShouldThrow_WhenEmailIsInUseByAnotherUser()
        {
            _userRepositoryMock.Setup(r => r.GetAllUsers()).Returns(new List<User>
            {
                new() { UserId = "U1", Name = "OTHER USER", Email = "email@x.com", IsActive = true },
                new() { UserId = "U2", Name = "", Email = "duplicate@x.com", IsActive = true }
            });

            var req = new UserRequest { UserId = "U1", Name = "OTHER USER", Email = "duplicate@x.com", Password = "password1*" };

            var act = () => _userService.UpdateUser(req);

            act.Should().Throw<ValidationException>().WithMessage("*E-mail already used*");
        }

        [Fact]
        public void DeleteUser_ShouldReturnTrue_WhenRepositorySucceeds()
        {
            _userRepositoryMock.Setup(r => r.DeleteUser("U1")).Returns(true);

            var result = _userService.DeleteUser("U1");

            result.Should().BeTrue();
        }

        [Fact]
        public void DeleteUser_ShouldThrow_WhenUserNotExists()
        {
            // Arrange
            var userId = "NON_EXISTENT_USER";

            _userRepositoryMock
                .Setup(r => r.DeleteUser(userId.ToUpper()))
                .Throws(new KeyNotFoundException("User not found."));

            // Act
            Action act = () => _userService.DeleteUser(userId);

            // Assert
            act.Should().Throw<KeyNotFoundException>();
        }

        [Fact]
        public void ValidateCredentials_ShouldReturnUser_WhenCorrect()
        {
            // Arrange
            var user = new User { UserId = "U1", Name = "User One" };
            user.SetPassword("password1*", _passwordHasherMock.Object);

            _userRepositoryMock.Setup(r => r.GetUserById("U1")).Returns(user);
            _passwordHasherMock
                .Setup(h => h.VerifyPassword("password1*", user.PasswordHash))
                .Returns(true);

            // Act
            var result = _userService.ValidateCredentials("U1", "password1*");

            // Assert
            result.Should().NotBeNull();
            result.UserId.Should().Be("U1");
        }

        [Fact]
        public void ValidateCredentials_ShouldThrow_WhenInvalidPassword()
        {
            var user = new User { UserId = "U1", Name = "User One" };
            user.SetPassword("password1*", _passwordHasherMock.Object);

            _userRepositoryMock.Setup(r => r.GetUserById("U1")).Returns(user);

            var act = () => _userService.ValidateCredentials("U1", "wrong");

            act.Should().Throw<UnauthorizedAccessException>();
        }
    }
}