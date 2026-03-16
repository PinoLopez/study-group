using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
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

        [Test]
        public async Task GetStudyGroups_WithSortAsc_ReturnsSortedByDateAsc()
        {
            var group1 = new StudyGroup(0, "Old Group", Subject.Math,
                DateTime.UtcNow.AddDays(-2), new System.Collections.Generic.List<User>());
            var group2 = new StudyGroup(0, "New Group", Subject.Chemistry,
                DateTime.UtcNow, new System.Collections.Generic.List<User>());

            _context.StudyGroups.AddRange(group1, group2);
            await _context.SaveChangesAsync();

            var result = await _repo.GetStudyGroups("asc");

            Assert.That(result.First().Name, Is.EqualTo("Old Group"));
        }

        [Test]
        public async Task SearchStudyGroups_BySubject_ReturnsFiltered()
        {
            var group1 = new StudyGroup(0, "Math Group", Subject.Math,
                DateTime.UtcNow, new System.Collections.Generic.List<User>());
            var group2 = new StudyGroup(0, "Chem Group", Subject.Chemistry,
                DateTime.UtcNow, new System.Collections.Generic.List<User>());

            _context.StudyGroups.AddRange(group1, group2);
            await _context.SaveChangesAsync();

            var result = await _repo.SearchStudyGroups("Math");

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.First().Subject, Is.EqualTo(Subject.Math));
        }
    }
}