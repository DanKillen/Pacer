using Moq;
using Pacer.Data.Entities;
using Pacer.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System;

public static class MockDatabaseSetup
{
    public static Mock<DbSet<T>> CreateMockSet<T>(List<T> data) where T : class
    {
        var mockSet = new Mock<DbSet<T>>();
        mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.AsQueryable().Provider);
        mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.AsQueryable().Expression);
        mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.AsQueryable().ElementType);
        mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.AsQueryable().GetEnumerator());
        return mockSet;
    }

    public static List<User> GetMockUsers()
    {
        return new List<User>
        {
            new() { Id = 1, Name = "John", Email = "john@mail.com" },
            new() { Id = 2, Name = "Jane", Email = "jane@mail.com" },
            new() { Id = 3, Name = "Jack", Email = "jack@mail.com" }
        };
    }

    public static List<RunningProfile> GetMockRunningProfiles()
    {
        var users = GetMockUsers();  // Get the mock users

        return new List<RunningProfile>
        {
            new() { Id = 1, UserId = 1, User = users[0], DateOfBirth = new DateTime(1990, 1, 1), Gender = "Male", WeeklyMileage = 20, FiveKTime = TimeSpan.FromMinutes(20) },
            new() { Id = 2, UserId = 2, User = users[1], DateOfBirth = new DateTime(1991, 1, 1), Gender = "Female", WeeklyMileage = 25, FiveKTime = TimeSpan.FromMinutes(22) },
        };
    }

    public static List<TrainingPlan> GetMockTrainingPlans()
    {
        var runningProfiles = GetMockRunningProfiles();  // Get the mock running profiles
        return new List<TrainingPlan>
        {
            new() { RunningProfileId = runningProfiles[0].Id, TargetRace = RaceType.Marathon, RaceDate = new DateTime(2024,1,31), TargetTime = TimeSpan.FromHours(4)  }// Your mock TrainingPlan objects here
        };
    }
}
