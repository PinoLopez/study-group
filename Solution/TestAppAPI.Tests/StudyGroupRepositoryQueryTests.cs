using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestApp;
using TestAppAPI;

namespace TestAppAPI.Tests
{
    [TestFixture]
    public class StudyGroupRepositoryQueryTests
    {
        private AppDbContext _context;
        private StudyGroupRepository _repo;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new AppDbContext(options);
            _repo = new StudyGroupRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        // Test Case 18: SQL query — all StudyGroups with at least one user whose
        // name starts with 'M', sorted by creation date ascending.
        // SQL equivalent:
        //   SELECT * FROM StudyGroups
        //   WHERE EXISTS (
        //       SELECT 1 FROM StudyGroupUsers su
        //       JOIN Users u ON u.Id = su.UsersId
        //       WHERE su.StudyGroupsStudyGroupId = StudyGroups.StudyGroupId
        //       AND u.Name LIKE 'M%'
        //   )
        //   ORDER BY CreateDate ASC;
        [Test]
        public async Task GetStudyGroupsWithUsersNamedStartingWithM_ReturnsOnlyMatchingGroupsSortedByDate()
        {
            // Arrange — seed users
            var miguel = new User(1, "Miguel Perez");
            var manuel = new User(2, "Manuel Pino");
            var john   = new User(3, "John Smith");

            _context.Users.AddRange(miguel, manuel, john);
            await _context.SaveChangesAsync();

            // Seed groups with known creation dates
            var olderGroup = new StudyGroup(
                studyGroupId: 0,
                name: "Math Study Club",
                subject: Subject.Math,
                createDate: DateTime.UtcNow.AddDays(-2),
                users: new List<User> { miguel, manuel }   // both start with M → should appear
            );

            var newerGroup = new StudyGroup(
                studyGroupId: 0,
                name: "Physics Fundamentals",
                subject: Subject.Physics,
                createDate: DateTime.UtcNow.AddDays(-1),
                users: new List<User> { miguel, john }     // one M-user → should appear
            );

            var excludedGroup = new StudyGroup(
                studyGroupId: 0,
                name: "Chem Basics",
                subject: Subject.Chemistry,
                createDate: DateTime.UtcNow,
                users: new List<User> { john }             // no M-users → must NOT appear
            );

            _context.StudyGroups.AddRange(olderGroup, newerGroup, excludedGroup);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repo.GetStudyGroupsWithUsersNamedStartingWithM();

            // Assert — only the two groups containing M-named users are returned
            Assert.That(result.Count, Is.EqualTo(2),
                "Only groups with at least one user whose name starts with 'M' should be returned.");

            // Assert — sorted by creation date ascending (oldest first)
            Assert.That(result[0].Name, Is.EqualTo("Math Study Club"),
                "Oldest group should appear first (ASC sort by CreateDate).");
            Assert.That(result[1].Name, Is.EqualTo("Physics Fundamentals"),
                "Newest qualifying group should appear second.");

            // Assert — excluded group is not present
            Assert.That(result.Any(g => g.Name == "Chem Basics"), Is.False,
                "Group with no M-named users must not be returned.");
        }
    }
}