using Xunit;
using Pacer.Data.Services;
using Moq;
using Pacer.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pacer.Data.Repositories;

namespace Pacer.Test
{
    public class UserTests
    {
        private readonly IUserService _userService;
        private readonly Mock<DbSet<User>> _mockSet;
        private readonly Mock<DatabaseContext> _mockContext;

        public UserTests()
        {
            _mockSet = new Mock<DbSet<User>>();
            _mockContext = new Mock<DatabaseContext>();
            _mockContext.Setup(ctx => ctx.Set<User>()).Returns(_mockSet.Object);

            _userService = new UserServiceDb(_mockContext.Object);
        }

        [Fact]
        public void GetUsers_ShouldReturnAllUsers()
        {
            // Arrange
            var mockUsers = new List<User> 
            {
                new User { Id = 1, Name = "John", Email = "john@mail.com" },
                new User { Id = 2, Name = "Jane", Email = "jane@mail.com" }
            }.AsQueryable();
            SetupMockSet(mockUsers);

            // Act
            var users = _userService.GetUsers();

            // Assert
            Assert.Equal(2, users.Count);
        }

        [Fact]
        public void GetUserById_WhenUserExists_ShouldReturnUser()
        {
            // Arrange
            var mockUsers = new List<User> 
            {
                new User { Id = 1, Name = "John", Email = "john@mail.com" }
            }.AsQueryable();
            SetupMockSet(mockUsers);

            // Act
            var user = _userService.GetUser(1);

            // Assert
            Assert.NotNull(user);
            Assert.Equal("John", user.Name);
        }

        // ... Further tests for other methods ...

        private void SetupMockSet(IQueryable<User> mockUsers)
        {
            _mockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(mockUsers.Provider);
            _mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(mockUsers.Expression);
            _mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(mockUsers.ElementType);
            _mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(() => mockUsers.GetEnumerator());
        }
    }
}
