using Microsoft.AspNetCore.Mvc;
using Moq;
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
    public class StudyGroupControllerTests
    {
        private Mock<IStudyGroupRepository> _mockRepo = new();
        private StudyGroupController _controller = null!;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IStudyGroupRepository>();
            _controller = new StudyGroupController(_mockRepo.Object);
            Console.WriteLine($"🔄 START: {TestContext.CurrentContext.Test.Name}");
        }

        [TearDown]
        public void TearDown()
        {
            var result = TestContext.CurrentContext.Result;
            var outcome = result.Outcome.Status switch
            {
                NUnit.Framework.Interfaces.TestStatus.Passed => "✅ PASSED",
                NUnit.Framework.Interfaces.TestStatus.Failed => "❌ FAILED",
                NUnit.Framework.Interfaces.TestStatus.Skipped => "⏭️ SKIPPED",
                NUnit.Framework.Interfaces.TestStatus.Inconclusive => "⚠️ INCONCLUSIVE",
                _ => $"⚠️ {result.Outcome.Status}"
            };
            Console.WriteLine($"{outcome}: {TestContext.CurrentContext.Test.Name}");
        }

        #region Create Study Group Tests

        [Test]
        public async Task CreateStudyGroup_ValidModel_ReturnsOk()
        {
            var group = new StudyGroup(0, "Math Masters", Subject.Math, DateTime.UtcNow, new());

            _mockRepo
                .Setup(r => r.CreateStudyGroup(It.IsAny<StudyGroup>()))
                .Returns(Task.CompletedTask);

            var result = await _controller.CreateStudyGroup(group);

            Assert.IsInstanceOf<OkResult>(result);
            _mockRepo.Verify(r => r.CreateStudyGroup(It.IsAny<StudyGroup>()), Times.Once);
        }

        [Test]
        public async Task CreateStudyGroup_DuplicateSubject_ReturnsBadRequest()
        {
            var group = new StudyGroup(0, "Chem Team", Subject.Chemistry, DateTime.UtcNow, new());

            _mockRepo
                .Setup(r => r.CreateStudyGroup(It.IsAny<StudyGroup>()))
                .ThrowsAsync(new InvalidOperationException("A study group for subject 'Chemistry' already exists."));

            var actionResult = await _controller.CreateStudyGroup(group);
            var result = actionResult as BadRequestObjectResult;

            Assert.IsNotNull(result);
            var message = result!.Value?.ToString() ?? string.Empty;
            Assert.That(message, Does.Contain("Chemistry"));
        }

        #endregion

        #region Get Study Groups Tests

        [Test]
        public async Task GetStudyGroups_DefaultSort_ReturnsOkWithListDescending()
        {
            var groups = new List<StudyGroup>
            {
                new(1, "Calculus Club", Subject.Math, DateTime.UtcNow.AddDays(-5), new()),
                new(2, "Algebra Group", Subject.Math, DateTime.UtcNow, new())
            };

            _mockRepo
                .Setup(r => r.GetStudyGroups("desc"))
                .ReturnsAsync(groups);

            var actionResult = await _controller.GetStudyGroups("desc");
            var result = actionResult as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(groups, result!.Value);
        }

        [Test]
        public async Task GetStudyGroups_SortAscending_ReturnsOkWithListAscending()
        {
            var groups = new List<StudyGroup>
            {
                new(1, "Old Group", Subject.Math, DateTime.UtcNow.AddDays(-10), new()),
                new(2, "New Group", Subject.Physics, DateTime.UtcNow, new())
            };

            _mockRepo
                .Setup(r => r.GetStudyGroups("asc"))
                .ReturnsAsync(groups);

            var actionResult = await _controller.GetStudyGroups("asc");
            var result = actionResult as OkObjectResult;

            Assert.IsNotNull(result);
            var resultGroups = result!.Value as IEnumerable<StudyGroup>;
            Assert.IsNotNull(resultGroups);
            Assert.AreEqual(2, resultGroups!.Count());
        }

        #endregion

        #region Search Study Groups Tests

        [Test]
        public async Task SearchStudyGroups_ValidSubject_ReturnsOk()
        {
            var groups = new List<StudyGroup>
            {
                new(1, "Quantum Physics Club", Subject.Physics, DateTime.UtcNow, new())
            };

            _mockRepo
                .Setup(r => r.SearchStudyGroups("Physics", "desc"))
                .ReturnsAsync(groups);

            var actionResult = await _controller.SearchStudyGroups("Physics", "desc");
            var result = actionResult as OkObjectResult;

            Assert.IsNotNull(result);
            var value = result!.Value as IEnumerable<StudyGroup> ?? Array.Empty<StudyGroup>();
            Assert.IsNotEmpty(value);
            Assert.AreEqual(Subject.Physics, value.First().Subject);
        }

        [Test]
        public async Task SearchStudyGroups_WithSortOrder_ReturnsOkSorted()
        {
            var groups = new List<StudyGroup>
            {
                new(1, "Old Physics", Subject.Physics, DateTime.UtcNow.AddDays(-5), new()),
                new(2, "New Physics", Subject.Physics, DateTime.UtcNow, new())
            };

            _mockRepo
                .Setup(r => r.SearchStudyGroups("Physics", "asc"))
                .ReturnsAsync(groups);

            var actionResult = await _controller.SearchStudyGroups("Physics", "asc");
            var result = actionResult as OkObjectResult;

            Assert.IsNotNull(result);
            var value = result!.Value as IEnumerable<StudyGroup>;
            Assert.IsNotNull(value);
            Assert.AreEqual(2, value!.Count());
        }

        #endregion

        #region Join Study Group Tests

        [Test]
        public async Task JoinStudyGroup_ValidIds_ReturnsOk()
        {
            _mockRepo
                .Setup(r => r.JoinStudyGroup(1, 100, "Miguel"))
                .Returns(Task.CompletedTask);

            var result = await _controller.JoinStudyGroup(1, 100, "Miguel");

            Assert.IsInstanceOf<OkResult>(result);
            _mockRepo.Verify(r => r.JoinStudyGroup(1, 100, "Miguel"), Times.Once);
        }

        [Test]
        public async Task JoinStudyGroup_InvalidGroup_ReturnsNotFound()
        {
            _mockRepo
                .Setup(r => r.JoinStudyGroup(999, 100, "Manuel"))
                .ThrowsAsync(new ArgumentException("Study group not found"));

            var actionResult = await _controller.JoinStudyGroup(999, 100, "Manuel");
            var result = actionResult as NotFoundObjectResult;

            Assert.IsNotNull(result);
            var message = result!.Value?.ToString() ?? string.Empty;
            Assert.That(message, Does.Contain("not found"));
        }

        [Test]
        public async Task JoinStudyGroup_DuplicateUser_ReturnsBadRequest()
        {
            _mockRepo
                .Setup(r => r.JoinStudyGroup(1, 100, "Maria"))
                .ThrowsAsync(new InvalidOperationException("User is already a member of this study group."));

            var actionResult = await _controller.JoinStudyGroup(1, 100, "Maria");
            var result = actionResult as BadRequestObjectResult;

            Assert.IsNotNull(result);
            var message = result!.Value?.ToString() ?? string.Empty;
            Assert.That(message, Does.Contain("already a member"));
        }

        [Test]
        public async Task JoinStudyGroup_EmptyUserName_ReturnsBadRequest()
        {
            _mockRepo
                .Setup(r => r.JoinStudyGroup(1, 100, ""))
                .ThrowsAsync(new ArgumentException("User name is required."));

            var actionResult = await _controller.JoinStudyGroup(1, 100, "");
            var result = actionResult as BadRequestObjectResult;

            Assert.IsNotNull(result);
        }

        #endregion

        #region Leave Study Group Tests

        [Test]
        public async Task LeaveStudyGroup_ValidIds_ReturnsOk()
        {
            _mockRepo
                .Setup(r => r.LeaveStudyGroup(1, 100))
                .Returns(Task.CompletedTask);

            var result = await _controller.LeaveStudyGroup(1, 100);

            Assert.IsInstanceOf<OkResult>(result);
            _mockRepo.Verify(r => r.LeaveStudyGroup(1, 100), Times.Once);
        }

        [Test]
        public async Task LeaveStudyGroup_UserNotInGroup_ReturnsNotFound()
        {
            _mockRepo
                .Setup(r => r.LeaveStudyGroup(1, 999))
                .ThrowsAsync(new ArgumentException("User is not a member of this study group."));

            var actionResult = await _controller.LeaveStudyGroup(1, 999);
            var result = actionResult as NotFoundObjectResult;

            Assert.IsNotNull(result);
            var message = result!.Value?.ToString() ?? string.Empty;
            Assert.That(message, Does.Contain("not a member"));
        }

        [Test]
        public async Task LeaveStudyGroup_InvalidGroup_ReturnsNotFound()
        {
            _mockRepo
                .Setup(r => r.LeaveStudyGroup(999, 100))
                .ThrowsAsync(new ArgumentException("Study group not found."));

            var actionResult = await _controller.LeaveStudyGroup(999, 100);
            var result = actionResult as NotFoundObjectResult;

            Assert.IsNotNull(result);
        }

        #endregion
    }
}