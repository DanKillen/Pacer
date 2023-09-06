using Xunit;
using Pacer.Data.Services;
using Moq;
using Pacer.Data.Entities;
using System;
using System.Linq;
using Pacer.Data.Repositories;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Pacer.Test
{
    public class RunningProfileTests
    {
        private readonly IRunningProfileService _runningProfileService;
        private readonly Mock<IDatabaseContext> _mockContext;

        public RunningProfileTests()
        {
            // Get mock data for RunningProfiles and Users
            var mockRunningProfileData = MockDatabaseSetup.GetMockRunningProfiles();
            var mockUserData = MockDatabaseSetup.GetMockUsers();

            // Create mock DbSets
            var mockRunningProfileSet = MockDatabaseSetup.CreateMockSet(mockRunningProfileData);
            var mockUserSet = MockDatabaseSetup.CreateMockSet(mockUserData);

            // Mock the DbContext
            _mockContext = new Mock<IDatabaseContext>();
            _mockContext.Setup(c => c.RunningProfiles).Returns(mockRunningProfileSet.Object);
            _mockContext.Setup(c => c.Users).Returns(mockUserSet.Object);  // Mocking Users DbSet
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);  // Return 1 to indicate one change was made

            var mockRaceTimePredictor = new Mock<IRaceTimePredictor>();
            mockRaceTimePredictor.Setup(r => r.CalculateEstimatedMarathonTime(It.IsAny<int>(), It.IsAny<TimeSpan>(), It.IsAny<string>())).Returns(TimeSpan.FromHours(4));
            mockRaceTimePredictor.Setup(r => r.CalculateEstimatedHalfMarathonTime(It.IsAny<int>(), It.IsAny<TimeSpan>(), It.IsAny<string>())).Returns(TimeSpan.FromMinutes(120));

            // Initialize the service to be tested, injecting the mock context and mock race time predictor
            _runningProfileService = new RunningProfileServiceDb(_mockContext.Object, mockRaceTimePredictor.Object);
        }

        [Fact]
        public void CreateProfile_ShouldCreateNewProfile()
        {
            // Arrange
            var newProfile = new RunningProfile
            {
                UserId = 3,
                DateOfBirth = new DateTime(1985, 1, 1),
                Gender = "Male",
                WeeklyMileage = 30,
                FiveKTime = TimeSpan.FromMinutes(18)
            };

            // Act
            var createdProfile = _runningProfileService.CreateProfile(newProfile.UserId, newProfile.DateOfBirth, newProfile.Gender, newProfile.WeeklyMileage, newProfile.FiveKTime);

            // Assert
            Assert.NotNull(createdProfile);
            Assert.Equal(newProfile.UserId, createdProfile.UserId);
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [Fact]
        public void GetProfileByUserId_ShouldReturnProfile()
        {
            // Act
            var profile = _runningProfileService.GetProfileByUserId(1);

            // Assert
            Assert.NotNull(profile);
            Assert.Equal(1, profile.UserId);
        }

        [Fact]
        public void GetProfileByUserId_InvalidUserId_ShouldReturnNull()
        {
            // Act
            var profile = _runningProfileService.GetProfileByUserId(-1);

            // Assert
            Assert.Null(profile);
        }

        [Fact]
        public void UpdateProfile_ShouldUpdateProfileDetails()
        {
            // Arrange

            // Act
            var result = _runningProfileService.UpdateProfile(1, new DateTime(1990, 1, 1), "Male", 25, TimeSpan.FromMinutes(19));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(25, result.WeeklyMileage);
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [Fact]
        public void CreateProfile_ForNonExistentUserId_ShouldReturnNull()
        {
            // Arrange
            var existingUserId = 99;
            var newProfile = new RunningProfile
            {
                UserId = existingUserId,
                DateOfBirth = new DateTime(1985, 1, 1),
                Gender = "Male",
                WeeklyMileage = 30,
                FiveKTime = TimeSpan.FromMinutes(18)
            };

            // Act
            var createdProfile = _runningProfileService.CreateProfile(newProfile.UserId, newProfile.DateOfBirth, newProfile.Gender, newProfile.WeeklyMileage, newProfile.FiveKTime);

            // Assert
            Assert.Null(createdProfile);
            _mockContext.Verify(m => m.SaveChanges(), Times.Never());
        }

        [Fact]
        public void CreateProfile_InvalidDateOfBirth_ShouldReturnError()
        {
            // Arrange
            var newProfile = new RunningProfile
            {
                UserId = 3,
                DateOfBirth = new DateTime(1800, 1, 1),
                Gender = null,
                WeeklyMileage = 30,
                FiveKTime = TimeSpan.FromMinutes(18)
            };

            Assert.Throws<ArgumentException>(() =>
                _runningProfileService.CreateProfile(newProfile.UserId, newProfile.DateOfBirth, newProfile.Gender, newProfile.WeeklyMileage, newProfile.FiveKTime)
            );

            _mockContext.Verify(m => m.SaveChanges(), Times.Never());
        }

        [Fact]
        public void CreateProfile_InvalidMileage_ShouldThrowError()
        {
            // Arrange
            var newProfile = new RunningProfile
            {
                UserId = 3,
                DateOfBirth = new DateTime(2000, 1, 1),
                Gender = null,
                WeeklyMileage = 300,
                FiveKTime = TimeSpan.FromMinutes(18)
            };

            Assert.Throws<ArgumentException>(() =>
                _runningProfileService.CreateProfile(newProfile.UserId, newProfile.DateOfBirth, newProfile.Gender, newProfile.WeeklyMileage, newProfile.FiveKTime)
            );

            _mockContext.Verify(m => m.SaveChanges(), Times.Never());
        }
        [Fact]
        public void CreateProfile_NegativeMileage_ShouldThrowError()
        {
            // Arrange
            var newProfile = new RunningProfile
            {
                UserId = 3,
                DateOfBirth = new DateTime(2000, 1, 1),
                Gender = null,
                WeeklyMileage = -1,
                FiveKTime = TimeSpan.FromMinutes(18)
            };

            Assert.Throws<ArgumentException>(() =>
                _runningProfileService.CreateProfile(newProfile.UserId, newProfile.DateOfBirth, newProfile.Gender, newProfile.WeeklyMileage, newProfile.FiveKTime)
            );

            _mockContext.Verify(m => m.SaveChanges(), Times.Never());
        }
        [Fact]
        public void CreateProfile_5KTimeTooSlow_ShouldThrowError()
        {
            // Arrange
            var newProfile = new RunningProfile
            {
                UserId = 3,
                DateOfBirth = new DateTime(2000, 1, 1),
                Gender = null,
                WeeklyMileage = 100,
                FiveKTime = TimeSpan.FromMinutes(200)
            };

            Assert.Throws<ArgumentException>(() =>
                _runningProfileService.CreateProfile(newProfile.UserId, newProfile.DateOfBirth, newProfile.Gender, newProfile.WeeklyMileage, newProfile.FiveKTime)
            );

            _mockContext.Verify(m => m.SaveChanges(), Times.Never());
        }
        [Fact]
        public void CreateProfile_5KTimeTooFast_ShouldThrowError()
        {
            // Arrange
            var newProfile = new RunningProfile
            {
                UserId = 3,
                DateOfBirth = new DateTime(2000, 1, 1),
                Gender = null,
                WeeklyMileage = 100,
                FiveKTime = TimeSpan.FromMinutes(10)
            };

            Assert.Throws<ArgumentException>(() =>
                _runningProfileService.CreateProfile(newProfile.UserId, newProfile.DateOfBirth, newProfile.Gender, newProfile.WeeklyMileage, newProfile.FiveKTime)
            );

            _mockContext.Verify(m => m.SaveChanges(), Times.Never());
        }

    }


}


