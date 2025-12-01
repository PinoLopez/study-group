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
        private AppDbContext _context = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite("DataSource=:memory:")
                .Options;

            _context = new AppDbContext(options);
            _context.Database.OpenConnection();
            _context.Database.EnsureCreated();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test(Description = "Verifies SQL query returns StudyGroups with users starting with 'M', sorted by CreateDate ASC.")]
        public async Task Query_ReturnsStudyGroupsWithUsersStartingWithM_SortedByCreateDate()
        {
            // Arrange: Insert test data
            var mathGroup = new StudyGroup(0, "Math Group", Subject.Math, new DateTime(2023, 1, 1), new List<User>());
            var chemGroup = new StudyGroup(0, "Chem Group", Subject.Chemistry, new DateTime(2023, 2, 1), new List<User>());
            var physicsGroup = new StudyGroup(0, "Physics Group", Subject.Physics, new DateTime(2023, 3, 1), new List<User>());

            var miguel = new User(1, "Miguel");
            var alice = new User(2, "Alice");
            var manuel = new User(3, "Manuel");
            var bob = new User(4, "Bob");

            mathGroup.AddUser(miguel);
            mathGroup.AddUser(alice);
            chemGroup.AddUser(manuel);
            physicsGroup.AddUser(bob);

            _context.StudyGroups.AddRange(mathGroup, chemGroup, physicsGroup);
            _context.Users.AddRange(miguel, alice, manuel, bob);
            await _context.SaveChangesAsync();

            // Act: Execute the raw SQL query with correct junction columns
            var query = @"
                SELECT
                    sg.StudyGroupId,
                    sg.Name,
                    sg.Subject,
                    sg.CreateDate
                FROM
                    StudyGroups sg
                INNER JOIN
                    StudyGroupUsers sgu ON sg.StudyGroupId = sgu.StudyGroupsStudyGroupId
                INNER JOIN
                    Users u ON sgu.UsersId = u.Id
                WHERE
                    u.Name LIKE 'M%'
                GROUP BY
                    sg.StudyGroupId, sg.Name, sg.Subject, sg.CreateDate
                ORDER BY
                    sg.CreateDate ASC;";

            var results = await _context.StudyGroups
                .FromSqlRaw(query)
                .ToListAsync();

            // Assert: Expected 2 groups (Math and Chem), sorted oldest first
            Assert.That(results.Count, Is.EqualTo(2));
            Assert.That(results[0].Name, Is.EqualTo("Math Group"));
            Assert.That(results[0].Subject, Is.EqualTo(Subject.Math));
            Assert.That(results[0].CreateDate, Is.EqualTo(new DateTime(2023, 1, 1)));

            Assert.That(results[1].Name, Is.EqualTo("Chem Group"));
            Assert.That(results[1].Subject, Is.EqualTo(Subject.Chemistry));
            Assert.That(results[1].CreateDate, Is.EqualTo(new DateTime(2023, 2, 1)));
        }
    }
}