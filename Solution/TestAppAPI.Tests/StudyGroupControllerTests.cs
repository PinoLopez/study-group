using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestApp;
using TestAppAPI;

namespace TestAppAPI.Tests
{
    [TestFixture]
    public class StudyGroupControllerTests
    {
        private Mock<IStudyGroupRepository> _mockRepo = new();
        private StudyGroupController _controller = null!;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IStudyGroupRepository>();
            _controller = new StudyGroupController(_mockRepo.Object);
        }

        [Test]
        public async Task CreateStudyGroup_ValidModel_ReturnsOk()
        {
            var group = new StudyGroup(0, "Math Masters", Subject.Math, DateTime.UtcNow, new());

            _mockRepo
                .Setup(r => r.CreateStudyGroup(It.IsAny<StudyGroup>()))
                .Returns(Task.CompletedTask);

            var result = await _controller.CreateStudyGroup(group);

            Assert.IsInstanceOf<OkResult>(result);
        }

        [Test]
        public async Task CreateStudyGroup_DuplicateSubject_ReturnsBadRequest()
        {
            var group = new StudyGroup(0, "Chem Team", Subject.Chemistry, DateTime.UtcNow, new());

            _mockRepo
                .Setup(r => r.CreateStudyGroup(It.IsAny<StudyGroup>()))
                .ThrowsAsync(new InvalidOperationException("Study group for Chemistry already exists"));

            var actionResult = await _controller.CreateStudyGroup(group);
            var result = actionResult as BadRequestObjectResult;

            Assert.IsNotNull(result);
            var message = result!.Value?.ToString() ?? string.Empty;
            Assert.That(message, Does.Contain("Chemistry"));
        }

        [Test]
        public async Task GetStudyGroups_ReturnsOkWithList()
        {
            var groups = new List<StudyGroup> { new(1, "Calculus Club", Subject.Math, DateTime.UtcNow, new()) };
            _mockRepo
                .Setup(r => r.GetStudyGroups())
                .ReturnsAsync(groups);

            var actionResult = await _controller.GetStudyGroups();
            var result = actionResult as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(groups, result!.Value);
        }

        [Test]
        public async Task SearchStudyGroups_ValidSubject_ReturnsOk()
        {
            var groups = new List<StudyGroup> { new(1, "Quantum Physics Club", Subject.Physics, DateTime.UtcNow, new()) };
            _mockRepo
                .Setup(r => r.SearchStudyGroups("Physics"))
                .ReturnsAsync(groups);

            var actionResult = await _controller.SearchStudyGroups("Physics");
            var result = actionResult as OkObjectResult;

            Assert.IsNotNull(result);

            var value = result!.Value as IEnumerable<StudyGroup> ?? Array.Empty<StudyGroup>();
            Assert.IsNotEmpty(value);
        }

        [Test]
        public async Task JoinStudyGroup_ValidIds_ReturnsOk()
        {
            _mockRepo
                .Setup(r => r.JoinStudyGroup(1, 100))
                .Returns(Task.CompletedTask);

            var result = await _controller.JoinStudyGroup(1, 100);

            Assert.IsInstanceOf<OkResult>(result);
        }

        [Test]
        public async Task JoinStudyGroup_InvalidGroup_ReturnsNotFound()
        {
            _mockRepo
                .Setup(r => r.JoinStudyGroup(999, 100))
                .ThrowsAsync(new ArgumentException("Study group not found"));

            var actionResult = await _controller.JoinStudyGroup(999, 100);
            var result = actionResult as NotFoundObjectResult;

            Assert.IsNotNull(result);
            var message = result!.Value?.ToString() ?? string.Empty;
            Assert.That(message, Does.Contain("not found"));
        }

        [Test]
        public async Task LeaveStudyGroup_ValidIds_ReturnsOk()
        {
            _mockRepo
                .Setup(r => r.LeaveStudyGroup(1, 100))
                .Returns(Task.CompletedTask);

            var result = await _controller.LeaveStudyGroup(1, 100);

            Assert.IsInstanceOf<OkResult>(result);
        }
    }
}
