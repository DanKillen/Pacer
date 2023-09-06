using Xunit;
using Moq;
using Pacer.Data.Services;
using Pacer.Data.Entities;
using Pacer.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Pacer.Test
{
    public class UserTests
    {
        private readonly IUserService _userService;
        private readonly Mock<IDatabaseContext> _mockContext;

        public UserTests()
        {
            // Get mock data
            var mockUserData = MockDatabaseSetup.GetMockUsers();

            // Create mock DbSet
            var mockSet = MockDatabaseSetup.CreateMockSet(mockUserData);

            // Mock the DbContext
            _mockContext = new Mock<IDatabaseContext>(); // Use DatabaseContext directly
            _mockContext.Setup(c => c.Users).Returns(mockSet.Object);

            // Initialize the service to be tested, injecting the mock context
            _userService = new UserServiceDb(_mockContext.Object);

        }

        [Fact]
        public void GetUsers_ShouldReturnAllUsers()
        {
            // Arrange
            // Already completed in the constructor

            // Act
            var users = _userService.GetUsers();

            // Assert
            Assert.Equal(3, users.Count);
        }

        [Fact]
        public void GetUserById_WhenUserExists_ShouldReturnUser()
        {
            // Arrange

            // Act
            var user = _userService.GetUser(1);

            // Assert
            Assert.NotNull(user);
            Assert.Equal("John", user.Name);
        }

        [Fact]
        public void AddUser_ShouldAddNewUser()
        {
            // Arrange
            var name = "Alice";
            var email = "alice123@mail.com";
            var password = "password";

            // Act
            var newUser = _userService.AddUser(name, email, password);

            // Assert
            Assert.NotNull(newUser);
            Assert.Equal("Alice", newUser.Name);
        }

        [Fact]
        public void DeleteUser_ShouldReturnTrue()
        {
            var result = _userService.DeleteUser(1);
            Assert.True(result);
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [Fact]
        public void UpdateUser_ShouldReturnUpdatedUser()
        {
            var updatedUser = new User { Id = 1, Name = "John Updated", Email = "johnupdated@mail.com", Password = "password" };
            var result = _userService.UpdateUser(updatedUser);
            Assert.NotNull(result);
            Assert.Equal("John Updated", result.Name);
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }
        
        [Fact]
        public void GetUserById_WhenUserDoesNotExist_ShouldReturnNull()
        {
            // Arrange

            // Act
            var user = _userService.GetUser(10); // Using an ID that does not exist

            // Assert
            Assert.Null(user);
        }

        [Fact]
        public void AddUser_WithExistingEmail_ShouldReturnNull()
        {
            // Arrange
            var name = "Alice";
            var email = "john@mail.com"; // Using an email that already exists
            var password = "password";

            // Act & Assert
            var newUser = _userService.AddUser(name, email, password);

            Assert.Null(newUser);
        }

        [Fact]
        public void DeleteUser_WhenUserDoesNotExist_ShouldReturnFalse()
        {
            // Arrange

            // Act
            var result = _userService.DeleteUser(10); // Using an ID that does not exist

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void UpdateUser_WhenUserDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            var updatedUser = new User { Id = 10, Name = "John Updated", Email = "johnupdated@mail.com", Password = "password" };

            // Act
            var result = _userService.UpdateUser(updatedUser);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void UpdateUser_WithExistingEmail_ShouldReturnNull()
        {
            // Arrange
            var existingUser = _userService.GetUser(1); // Get an existing user
            var updatedUser = new User { Id = 1, Name = "John Updated", Email = "jane@mail.com", Password = "password" };

            // Act 
            var result = _userService.UpdateUser(updatedUser);
            Assert.Null(result);
        }

    }
}
