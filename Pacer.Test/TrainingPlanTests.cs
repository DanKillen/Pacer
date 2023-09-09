using Xunit;
using Pacer.Data.Services;
using Moq;
using Pacer.Data.Entities;
using System;
using Microsoft.Extensions.Logging;
using Pacer.Data.Repositories;
using System.Collections.Generic;
using Pacer.Data.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace Pacer.Test
{
    public class TrainingPlanTests
    {
        private readonly ITrainingPlanService _trainingPlanService;
        private readonly Mock<IDatabaseContext> _mockContext;

        public TrainingPlanTests()
        {
            var mockTrainingPlans = MockDatabaseSetup.GetMockTrainingPlans();
            var mockRunningProfileData = MockDatabaseSetup.GetMockRunningProfiles();
            var mockUserData = MockDatabaseSetup.GetMockUsers();

            // Create mock DbSets
            var mockTrainingPlanSet = MockDatabaseSetup.CreateMockSet(mockTrainingPlans);
            var mockRunningProfileSet = MockDatabaseSetup.CreateMockSet(mockRunningProfileData);
            var mockUserSet = MockDatabaseSetup.CreateMockSet(mockUserData);

            // Mock Dependencies
            _mockContext = new Mock<IDatabaseContext>();
            var mockLogger = new Mock<ILogger<TrainingPlanServiceDb>>();
            var mockWorkoutFactory = new Mock<IWorkoutFactory>();
            var mockRunningProfileService = new Mock<IRunningProfileService>();
            mockRunningProfileService.Setup(s => s.GetProfileByProfileId(2)).Returns(mockRunningProfileData[1]);
            var mockWorkoutPaceCalculator = new Mock<IWorkoutPaceCalculator>();
            _ = mockWorkoutPaceCalculator.Setup(c => c.CalculatePaces(It.IsAny<TimeSpan>(), It.IsAny<RaceType>(), It.IsAny<int>(), It.IsAny<String>()))
                                    .Returns(new List<TrainingPlanPace>
                                    {
                                        new TrainingPlanPace { PaceType = PaceType.Min, Pace = new PaceTime(8, 0) },
                                        new TrainingPlanPace { PaceType = PaceType.Max, Pace = new PaceTime(8, 0) }
                                    });
            _mockContext.Setup(c => c.TrainingPlans).Returns(mockTrainingPlanSet.Object);
            _mockContext.Setup(c => c.RunningProfiles).Returns(mockRunningProfileSet.Object);
            _mockContext.Setup(c => c.Users).Returns(mockUserSet.Object);  // Mocking Users DbSet
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);  // Return 1 to indicate one change was made

            // Initialize Service
            _trainingPlanService = new TrainingPlanServiceDb(_mockContext.Object, mockLogger.Object, mockWorkoutFactory.Object, mockRunningProfileService.Object, mockWorkoutPaceCalculator.Object);
        }

        [Fact]
        public void CreatePlan_ShouldReturnNewTrainingPlan()
        {
            // Arrange
            int validProfileId = 2;
            // Act
            var newPlan = _trainingPlanService.CreatePlan(validProfileId, RaceType.Marathon, DateTime.Now, TimeSpan.FromHours(4));

            // Assert
            Assert.NotNull(newPlan);
            Assert.Equal(RaceType.Marathon, newPlan.TargetRace);
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [Fact]
        public void CreatePlan_WithNullRunningProfile_ShouldReturnNull()
        {
            // Arrange
            _mockContext.Setup(ctx => ctx.RunningProfiles.Find(It.IsAny<int>())).Returns((RunningProfile)null);

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                _trainingPlanService.CreatePlan(1, RaceType.Marathon, DateTime.Now, TimeSpan.FromHours(4))
            );

            // You can still verify that SaveChanges was never called, if that's relevant for this test
            _mockContext.Verify(m => m.SaveChanges(), Times.Never());
        }

        [Fact]
        public void CreatePlan_WithInvalidRunningProfileId_ShouldThrowException()
        {
            // Arrange
            const int invalidProfileId = 999;

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                _trainingPlanService.CreatePlan(invalidProfileId, RaceType.Marathon, DateTime.Now, TimeSpan.FromHours(4))
            );

            // Verify that SaveChanges was never called
            _mockContext.Verify(m => m.SaveChanges(), Times.Never());
        }

        [Fact]
        public void CreatePlan_WithNegativeTargetTime_ShouldThrowException()
        {
            // Arrange
            int validProfileId = 2;

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                _trainingPlanService.CreatePlan(validProfileId, RaceType.Marathon, DateTime.Now, TimeSpan.FromHours(-1))
            );

            // Verify that SaveChanges was never called
            _mockContext.Verify(m => m.SaveChanges(), Times.Never());
        }

        [Fact]
        public void EditTargetTime_InvalidPlanId_ShouldReturnFalse()
        {
            // Arrange

            // Act
            var result = _trainingPlanService.EditTargetTime(10, TimeSpan.FromHours(3));

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void GetRecommendation_AgeOver70_ShouldAdviseAgainstLongDistances()
        {
            // Arrange
            DateTime dob = DateTime.Now.AddYears(-75);

            // Act
            var result = _trainingPlanService.GetRecommendation(TimeSpan.FromHours(4), TimeSpan.FromHours(2), 30, dob, TimeSpan.FromMinutes(30));

            // Assert
            Assert.Contains("Given your age, we would advise against", result[2]);
        }

        [Fact]
        public void GetPlanById_PlanDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            _mockContext.Setup(ctx => ctx.TrainingPlans.Find(It.IsAny<int>())).Returns((TrainingPlan)null);

            // Act
            var plan = _trainingPlanService.GetPlanById(1);

            // Assert
            Assert.Null(plan);
        }
        [Fact]
        public void UpdatePlan_ValidPlan_ShouldReturnUpdatedPlan()
        {
            // Arrange
            var originalPlan = new TrainingPlan { Id = 1, TargetTime = TimeSpan.FromHours(4) };
            var updatedPlan = new TrainingPlan { Id = 1, TargetTime = TimeSpan.FromHours(3) };
            _mockContext.Setup(ctx => ctx.TrainingPlans.Update(It.IsAny<TrainingPlan>()));

            // Act
            var result = _trainingPlanService.UpdatePlan(updatedPlan);

            // Assert
            Assert.Equal(updatedPlan.TargetTime, result.TargetTime);
            _mockContext.Verify(ctx => ctx.SaveChanges(), Times.Once());
        }

        [Fact]
        public void DeletePlan_ValidPlan_ShouldReturnTrueAndSaveChanges()
        {
            // Arrange
            var planToBeDeleted = new TrainingPlan { Id = 1, TargetTime = TimeSpan.FromHours(4) };
            _mockContext.Setup(ctx => ctx.TrainingPlans.Remove(It.IsAny<TrainingPlan>())).Verifiable();

            // Act
            var result = _trainingPlanService.DeletePlan(planToBeDeleted);

            // Assert
            Assert.True(result);
            _mockContext.Verify(ctx => ctx.SaveChanges(), Times.Once());
        }

        [Fact]
        public void SaveWorkoutActuals_WorkoutDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            _mockContext.Setup(ctx => ctx.TrainingPlans.Find(It.IsAny<int>())).Returns((TrainingPlan)null);

            // Act
            var result = _trainingPlanService.SaveWorkoutActuals(1, 1, 5, TimeSpan.FromMinutes(30));

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ClearWorkoutActuals_WorkoutDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            _mockContext.Setup(ctx => ctx.TrainingPlans.Find(It.IsAny<int>())).Returns((TrainingPlan)null);

            // Act
            var result = _trainingPlanService.ClearWorkoutActuals(1, 1);

            // Assert
            Assert.False(result);
        }


    }
}