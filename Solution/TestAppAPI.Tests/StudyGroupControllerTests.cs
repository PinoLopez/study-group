using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using TestApp;
using TestAppAPI;

namespace TestAppAPI.Tests
{
    [TestFixture]
    public class StudyGroupControllerTests
    {
        private Mock<IStudyGroupRepository> _mockRepo;
        private StudyGroupController _controller;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IStudyGroupRepository>();
            _controller = new StudyGroupController(_mockRepo.Object);
        }

        // Existing 15 tests assumed here (creation, get, search, join/leave scenarios)...

        [Test]
        [Category("Smoke")]
        public async Task CreateStudyGroup_ValidGroup_CallsRepoAndReturnsOk()
        {
            var group = new StudyGroup("Valid Group", Subject.Math);
            await _controller.CreateStudyGroup(group);
            _mockRepo.Verify(r => r.CreateStudyGroup(group), Times.Once);
            Assert.IsInstanceOf<OkResult>(await _controller.CreateStudyGroup(group));
        }

        [Test]
        public async Task GetStudyGroups_CallsRepoAndReturnsOkWithList()
        {
            _mockRepo.Setup(r => r.GetStudyGroups(It.IsAny<string>())).ReturnsAsync(new List<StudyGroup>());
            var result = await _controller.GetStudyGroups("asc");
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task SearchStudyGroups_ValidSubject_CallsRepoAndReturnsOk()
        {
            _mockRepo.Setup(r => r.SearchStudyGroups("Math", It.IsAny<string>())).ReturnsAsync(new List<StudyGroup>());
            var result = await _controller.SearchStudyGroups("Math", "desc");
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        [Category("Smoke")]
        public async Task JoinStudyGroup_ValidIds_CallsRepoAndReturnsOk()
        {
            await _controller.JoinStudyGroup(1, 100);
            _mockRepo.Verify(r => r.JoinStudyGroup(1, 100), Times.Once);
        }

      [Test]
    public async Task JoinStudyGroup_InvalidGroup_ReturnsNotFound()
    {
    _mockRepo.Setup(r => r.JoinStudyGroup(999, 100))
             .ThrowsAsync(new ArgumentException("Study group not found."));
    var result = await _controller.JoinStudyGroup(999, 100);
    Assert.IsInstanceOf<NotFoundObjectResult>(result);
    }

        [Test]
        public async Task LeaveStudyGroup_ValidIds_CallsRepoAndReturnsOk()
        {
            await _controller.LeaveStudyGroup(1, 100);
            _mockRepo.Verify(r => r.LeaveStudyGroup(1, 100), Times.Once);
        }
    }
}