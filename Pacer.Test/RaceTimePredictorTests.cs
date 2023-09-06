using Xunit;
using Pacer.Utilities;
using System;
using Pacer.Data.Entities;

namespace Pacer.Test
{
    public class RaceTimePredictorTests
    {
        private readonly IRaceTimePredictor _raceTimePredictor;

        public RaceTimePredictorTests()
        {
            // Initialize the service to be tested
            _raceTimePredictor = new RaceTimePredictor();
        }

        [Fact]
        public void CalculateEstimatedMarathonTime_WhenOnlyGenderIsDifferent_ShouldReturnDifferentTimes()
        {
            // Arrange
            int age1 = 30;
            TimeSpan fiveKTime1 = TimeSpan.FromMinutes(25);
            string gender1 = "Male";

            int age2 = 30;
            TimeSpan fiveKTime2 = TimeSpan.FromMinutes(25);
            string gender2 = "Female";

            // Act
            var result1 = _raceTimePredictor.CalculateEstimatedMarathonTime(age1, fiveKTime1, gender1);
            var result2 = _raceTimePredictor.CalculateEstimatedMarathonTime(age2, fiveKTime2, gender2);

            // Assert
            Assert.NotEqual(result1, result2);
        }

        [Fact]
        public void CalculateEstimatedMarathonTime_WhenOnly5kIsDifferent_ShouldReturnDifferentTimes()
        {
            // Arrange
            int age1 = 30;
            TimeSpan fiveKTime1 = TimeSpan.FromMinutes(25);
            string gender1 = "Male";

            int age2 = 30;
            TimeSpan fiveKTime2 = TimeSpan.FromMinutes(24);
            string gender2 = "Male";

            // Act
            var result1 = _raceTimePredictor.CalculateEstimatedMarathonTime(age1, fiveKTime1, gender1);
            var result2 = _raceTimePredictor.CalculateEstimatedMarathonTime(age2, fiveKTime2, gender2);

            // Assert
            Assert.NotEqual(result1, result2);
        }

        [Fact]
        public void CalculateEstimatedMarathonTime_WhenOnlyAgeIsDifferent_ShouldReturnDifferentTimes()
        {
            // Arrange
            int age1 = 31;
            TimeSpan fiveKTime1 = TimeSpan.FromMinutes(25);
            string gender1 = "Male";

            int age2 = 30;
            TimeSpan fiveKTime2 = TimeSpan.FromMinutes(25);
            string gender2 = "Male";

            // Act
            var result1 = _raceTimePredictor.CalculateEstimatedMarathonTime(age1, fiveKTime1, gender1);
            var result2 = _raceTimePredictor.CalculateEstimatedMarathonTime(age2, fiveKTime2, gender2);

            // Assert
            Assert.NotEqual(result1, result2);
        }

        [Fact]
        public void CalculateEstimatedHalfMarathonTime_ShouldReturnEstimatedTime()
        {
            // Arrange
            int age = 25;
            TimeSpan fiveKTime = TimeSpan.FromMinutes(20);
            string gender = "Male";

            // From working out the math with CorrectionFactors->
            // 20 minutes for 5k at age 25 for a male should end up with 93 minutes for a half marathon
            // 5K Correction Factor: 0.997, Half Marathon Correction Factor: 0.911
            // Step 1: Calculate the original 5K pace
            // 20 minutes / 3.1 miles = 6.4516 minutes per mile
            // Step 2: Adjust the 5K pace with the 5K correction factor
            // 6.4516 / 0.997 = 6.4710 minutes per mile (corrected 5K pace)
            // Step 3: Estimate the raw half-marathon time using the corrected 5K pace
            // 6.4710 minutes per mile * 13.1094 miles = 84.8310 minutes
            // Step 4: Adjust the estimated half-marathon time with the half-marathon correction factor
            // 84.8310 minutes / 0.911 = 93.11 minutes (corrected half-marathon time)
            var expectedResult = 93;

            // Act
            var result = _raceTimePredictor.CalculateEstimatedHalfMarathonTime(age, fiveKTime, gender);

            // Assert
            Assert.Equal(expectedResult, (int)Math.Round(result.TotalMinutes));
        }

        [Fact]
        public void CalculateEquivalentMarathonPace_ShouldReturnEquivalentPace()
        {
            // Arrange
            TimeSpan targetTime = TimeSpan.FromHours(2);
            RaceType raceType = RaceType.HalfMarathon;
            int age = 30;
            string gender = "Male";
            // From working out the math with CorrectionFactors->
            // 2 hours for a 30 year old male running a half-marathon follows this process:
            // Half-marathon Correction Factor: 0.916, Marathon Correction Factor: 0.877
             
            // Step 1: Calculate the original Half-Marathon pace
            // 120 minutes / 13.1094 miles = 9.1537 minutes per mile

            // Step 2: Adjust the half-marathon pace with the correction factor
            // 9.1537 * 0.916 = 8.3926 minutes per mile (corrected half-marathon pace)

            // Step 4: Adjust the corrected half-marathon pace with marathon correction factor
            // 8.3926 / 0.877 = 9.5686 minutes per mile (corrected marathon pace)
            var expectedTime = TimeSpan.FromMinutes(9.5686);

            // Act
            var result = _raceTimePredictor.CalculateEquivalentMarathonPace(targetTime, raceType, age, gender);

            // Assert
           Assert.True(Math.Abs((expectedTime - result).TotalSeconds) < 1); // Assert that the difference is less than 1 second, solely due to rounding

        }

    }
}