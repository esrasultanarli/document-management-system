using Microsoft.EntityFrameworkCore;
using Moq;
using DocumentManagementSystem.Data;
using DocumentManagementSystem.Models;
using DocumentManagementSystem.Services;
using DocumentManagementSystem.ViewModels;
using Xunit;

namespace DocumentManagementSystem.Tests
{
    public class AuthServiceTests
    {
        private readonly ApplicationDbContext _context;
        private readonly AuthService _service;

        public AuthServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _service = new AuthService(_context);
        }

        [Fact]
        public async Task RegisterUserAsync_ValidUser_ShouldCreateUser()
        {
            // Arrange
            var model = new RegisterViewModel
            {
                Username = "testuser",
                Email = "test@example.com",
                FirstName = "Test",
                LastName = "User",
                Password = "TestPassword123!"
            };

            // Act
            var result = await _service.RegisterUserAsync(model);

            // Assert
            Assert.True(result);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == "testuser");
            Assert.NotNull(user);
            Assert.Equal("test@example.com", user.Email);
            Assert.Equal("Test", user.FirstName);
            Assert.Equal("User", user.LastName);
            Assert.True(user.IsActive);
            Assert.Equal("User", user.Role);
        }

        [Fact]
        public async Task RegisterUserAsync_DuplicateUsername_ShouldReturnFalse()
        {
            // Arrange
            var existingUser = new User
            {
                Username = "testuser",
                Email = "existing@example.com",
                PasswordHash = "hash",
                IsActive = true
            };
            _context.Users.Add(existingUser);
            await _context.SaveChangesAsync();

            var model = new RegisterViewModel
            {
                Username = "testuser",
                Email = "new@example.com",
                FirstName = "New",
                LastName = "User",
                Password = "TestPassword123!"
            };

            // Act
            var result = await _service.RegisterUserAsync(model);

            // Assert
            Assert.False(result);
            var userCount = await _context.Users.CountAsync(u => u.Username == "testuser");
            Assert.Equal(1, userCount);
        }

        [Fact]
        public async Task RegisterUserAsync_DuplicateEmail_ShouldReturnFalse()
        {
            // Arrange
            var existingUser = new User
            {
                Username = "existinguser",
                Email = "test@example.com",
                PasswordHash = "hash",
                IsActive = true
            };
            _context.Users.Add(existingUser);
            await _context.SaveChangesAsync();

            var model = new RegisterViewModel
            {
                Username = "newuser",
                Email = "test@example.com",
                FirstName = "New",
                LastName = "User",
                Password = "TestPassword123!"
            };

            // Act
            var result = await _service.RegisterUserAsync(model);

            // Assert
            Assert.False(result);
            var userCount = await _context.Users.CountAsync(u => u.Email == "test@example.com");
            Assert.Equal(1, userCount);
        }

        [Fact]
        public async Task AuthenticateUserAsync_ValidCredentials_ShouldReturnUser()
        {
            // Arrange
            var password = "TestPassword123!";
            var hashedPassword = _service.HashPassword(password);
            var user = new User
            {
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = hashedPassword,
                IsActive = true
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.AuthenticateUserAsync("testuser", password);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("testuser", result.Username);
            Assert.Equal("test@example.com", result.Email);
        }

        [Fact]
        public async Task AuthenticateUserAsync_InvalidPassword_ShouldReturnNull()
        {
            // Arrange
            var password = "TestPassword123!";
            var hashedPassword = _service.HashPassword(password);
            var user = new User
            {
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = hashedPassword,
                IsActive = true
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.AuthenticateUserAsync("testuser", "WrongPassword");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AuthenticateUserAsync_NonExistentUser_ShouldReturnNull()
        {
            // Act
            var result = await _service.AuthenticateUserAsync("nonexistent", "password");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AuthenticateUserAsync_InactiveUser_ShouldReturnNull()
        {
            // Arrange
            var password = "TestPassword123!";
            var hashedPassword = _service.HashPassword(password);
            var user = new User
            {
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = hashedPassword,
                IsActive = false
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.AuthenticateUserAsync("testuser", password);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserByIdAsync_ExistingUser_ShouldReturnUser()
        {
            // Arrange
            var user = new User
            {
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = "hash",
                IsActive = true
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetUserByIdAsync(user.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("testuser", result.Username);
        }

        [Fact]
        public async Task GetUserByIdAsync_NonExistentUser_ShouldReturnNull()
        {
            // Act
            var result = await _service.GetUserByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserByUsernameAsync_ExistingUser_ShouldReturnUser()
        {
            // Arrange
            var user = new User
            {
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = "hash",
                IsActive = true
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetUserByUsernameAsync("testuser");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("testuser", result.Username);
        }

        [Fact]
        public async Task GetUserByUsernameAsync_NonExistentUser_ShouldReturnNull()
        {
            // Act
            var result = await _service.GetUserByUsernameAsync("nonexistent");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateUserAsync_ValidUser_ShouldUpdateSuccessfully()
        {
            // Arrange
            var user = new User
            {
                Username = "testuser",
                Email = "test@example.com",
                FirstName = "Test",
                LastName = "User",
                PasswordHash = "hash",
                IsActive = true
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            user.FirstName = "Updated";
            user.LastName = "Name";

            // Act
            var result = await _service.UpdateUserAsync(user);

            // Assert
            Assert.True(result);
            var updatedUser = await _context.Users.FindAsync(user.Id);
            Assert.Equal("Updated", updatedUser.FirstName);
            Assert.Equal("Name", updatedUser.LastName);
        }

        [Fact]
        public async Task ChangePasswordAsync_ValidCurrentPassword_ShouldChangePassword()
        {
            // Arrange
            var currentPassword = "OldPassword123!";
            var newPassword = "NewPassword123!";
            var hashedPassword = _service.HashPassword(currentPassword);
            var user = new User
            {
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = hashedPassword,
                IsActive = true
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.ChangePasswordAsync(user.Id, currentPassword, newPassword);

            // Assert
            Assert.True(result);
            var updatedUser = await _context.Users.FindAsync(user.Id);
            Assert.True(_service.VerifyPassword(newPassword, updatedUser.PasswordHash));
        }

        [Fact]
        public async Task ChangePasswordAsync_InvalidCurrentPassword_ShouldReturnFalse()
        {
            // Arrange
            var currentPassword = "OldPassword123!";
            var newPassword = "NewPassword123!";
            var hashedPassword = _service.HashPassword(currentPassword);
            var user = new User
            {
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = hashedPassword,
                IsActive = true
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.ChangePasswordAsync(user.Id, "WrongPassword", newPassword);

            // Assert
            Assert.False(result);
            var updatedUser = await _context.Users.FindAsync(user.Id);
            Assert.True(_service.VerifyPassword(currentPassword, updatedUser.PasswordHash));
        }

        [Fact]
        public async Task IsUsernameAvailableAsync_AvailableUsername_ShouldReturnTrue()
        {
            // Act
            var result = await _service.IsUsernameAvailableAsync("newuser");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsUsernameAvailableAsync_TakenUsername_ShouldReturnFalse()
        {
            // Arrange
            var user = new User
            {
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = "hash",
                IsActive = true
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.IsUsernameAvailableAsync("testuser");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsEmailAvailableAsync_AvailableEmail_ShouldReturnTrue()
        {
            // Act
            var result = await _service.IsEmailAvailableAsync("new@example.com");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsEmailAvailableAsync_TakenEmail_ShouldReturnFalse()
        {
            // Arrange
            var user = new User
            {
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = "hash",
                IsActive = true
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.IsEmailAvailableAsync("test@example.com");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void HashPassword_ValidPassword_ShouldReturnHash()
        {
            // Arrange
            var password = "TestPassword123!";

            // Act
            var hash = _service.HashPassword(password);

            // Assert
            Assert.NotNull(hash);
            Assert.NotEqual(password, hash);
            Assert.True(hash.Length > 0);
        }

        [Fact]
        public void VerifyPassword_ValidPassword_ShouldReturnTrue()
        {
            // Arrange
            var password = "TestPassword123!";
            var hash = _service.HashPassword(password);

            // Act
            var result = _service.VerifyPassword(password, hash);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void VerifyPassword_InvalidPassword_ShouldReturnFalse()
        {
            // Arrange
            var password = "TestPassword123!";
            var hash = _service.HashPassword(password);

            // Act
            var result = _service.VerifyPassword("WrongPassword", hash);

            // Assert
            Assert.False(result);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}