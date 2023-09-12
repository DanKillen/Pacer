using Xunit;
using Moq;
using Pacer.Data.Entities;
using Pacer.Data.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Pacer.Data.Services;

namespace Pacer.Test
{
    public class WorkoutPaceCalculatorTests
    {
        private readonly WorkoutPaceCalculator _workoutPaceCalculator;
        private readonly Mock<IRaceTimePredictor> _mockRaceTimePredictor;

        public WorkoutPaceCalculatorTests()
        {
            // Mocking the RaceTimePredictor
            _mockRaceTimePredictor = new Mock<IRaceTimePredictor>();

            // Initialize the service under test
            _workoutPaceCalculator = new WorkoutPaceCalculator(_mockRaceTimePredictor.Object);
        }

        [Fact]
        public void CalculatePaces_ShouldReturnPaces()
        {
            // Arrange
            TimeSpan targetTime = TimeSpan.FromMinutes(120);
            RaceType raceType = RaceType.HalfMarathon;
            int age = 25;
            string gender = "Male";
            TimeSpan equivalentMarathonPace = TimeSpan.FromMinutes(6);

            // Setup mock
            _mockRaceTimePredictor.Setup(r => r.CalculateEquivalentMarathonPace(targetTime, raceType, age, gender))
                                   .Returns(equivalentMarathonPace);

            // Act
            var paces = _workoutPaceCalculator.CalculatePaces(targetTime, raceType, age, gender);

            // Assert
            Assert.NotNull(paces);
            Assert.True(paces.Count > 0);

        }

        [Fact]
        public void CalculatePaces_DifferentEntriesShouldNotBeEqual()
        {
            // Arrange
            TimeSpan targetTime1 = TimeSpan.FromMinutes(120);
            TimeSpan targetTime2 = TimeSpan.FromMinutes(130);
            RaceType raceType = RaceType.HalfMarathon;
            int age = 25;
            string gender = "Male";
            TimeSpan equivalentMarathonPace1 = TimeSpan.FromMinutes(6);
            TimeSpan equivalentMarathonPace2 = TimeSpan.FromMinutes(6.5);

            // Setup mock
            _mockRaceTimePredictor.Setup(r => r.CalculateEquivalentMarathonPace(targetTime1, raceType, age, gender))
                                   .Returns(equivalentMarathonPace1);
            _mockRaceTimePredictor.Setup(r => r.CalculateEquivalentMarathonPace(targetTime2, raceType, age, gender))
                                   .Returns(equivalentMarathonPace2);

            // Act
            var paces1 = _workoutPaceCalculator.CalculatePaces(targetTime1, raceType, age, gender);
            var paces2 = _workoutPaceCalculator.CalculatePaces(targetTime2, raceType, age, gender);

            // Assert
            Assert.NotEqual(paces1.First().Pace, paces2.First().Pace);
        }

    }
}