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
        private Mock<IStudyGroupRepository> _mockRepo;
        private StudyGroupController _controller;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IStudyGroupRepository>();
            _controller = new StudyGroupController(_mockRepo.Object);
        }

        // ── CREATION (3 tests) ────────────────────────────────────────────────

        [Test]
        [Category("Smoke")]
        public async Task CreateStudyGroup_ValidGroup_CallsRepoAndReturnsOk()
        {
            var group = new StudyGroup("Valid Group", Subject.Math);
            _mockRepo.Setup(r => r.CreateStudyGroup(group)).Returns(Task.CompletedTask);

            var result = await _controller.CreateStudyGroup(group);

            _mockRepo.Verify(r => r.CreateStudyGroup(group), Times.Once);
            Assert.IsInstanceOf<OkResult>(result);
        }

        [Test]
        public async Task CreateStudyGroup_NullGroup_ReturnsBadRequest()
        {
            var result = await _controller.CreateStudyGroup(null);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task CreateStudyGroup_DuplicateSubject_ReturnsBadRequest()
        {
            var group = new StudyGroup("Chem Team", Subject.Chemistry);
            _mockRepo.Setup(r => r.CreateStudyGroup(group))
                     .ThrowsAsync(new InvalidOperationException(
                         "A study group for subject 'Chemistry' already exists."));

            var result = await _controller.CreateStudyGroup(group);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task CreateStudyGroup_InvalidName_ReturnsBadRequest()
        {
            // Domain constructor throws ArgumentException for short/long names,
            // but the controller also handles ArgumentException from the repo.
            var group = new StudyGroup("Valid Group", Subject.Physics); // valid to construct
            _mockRepo.Setup(r => r.CreateStudyGroup(group))
                     .ThrowsAsync(new ArgumentException("Name must be 5-30 characters."));

            var result = await _controller.CreateStudyGroup(group);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        // ── GET ALL (2 tests) ─────────────────────────────────────────────────

        [Test]
        public async Task GetStudyGroups_SortAsc_ReturnsOkWithList()
        {
            _mockRepo.Setup(r => r.GetStudyGroups("asc"))
                     .ReturnsAsync(new List<StudyGroup>());

            var result = await _controller.GetStudyGroups("asc");

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task GetStudyGroups_SortDesc_ReturnsOkWithList()
        {
            _mockRepo.Setup(r => r.GetStudyGroups("desc"))
                     .ReturnsAsync(new List<StudyGroup>());

            var result = await _controller.GetStudyGroups("desc");

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        // ── SEARCH (3 tests) ──────────────────────────────────────────────────

        [Test]
        [Category("Smoke")]
        public async Task SearchStudyGroups_ValidSubject_ReturnsOk()
        {
            _mockRepo.Setup(r => r.SearchStudyGroups("Math", It.IsAny<string>()))
                     .ReturnsAsync(new List<StudyGroup>());

            var result = await _controller.SearchStudyGroups("Math", "desc");

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task SearchStudyGroups_InvalidSubject_ReturnsBadRequest()
        {
            _mockRepo.Setup(r => r.SearchStudyGroups("Biology", It.IsAny<string>()))
                     .ThrowsAsync(new ArgumentException("Invalid subject."));

            var result = await _controller.SearchStudyGroups("Biology", "desc");

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task SearchStudyGroups_WithSortDesc_ReturnsOk()
        {
            _mockRepo.Setup(r => r.SearchStudyGroups("Physics", "desc"))
                     .ReturnsAsync(new List<StudyGroup>());

            var result = await _controller.SearchStudyGroups("Physics", "desc");

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        // ── JOIN (3 tests) ────────────────────────────────────────────────────

        [Test]
        [Category("Smoke")]
        public async Task JoinStudyGroup_ValidIds_CallsRepoAndReturnsOk()
        {
            _mockRepo.Setup(r => r.JoinStudyGroup(1, 100)).Returns(Task.CompletedTask);

            var result = await _controller.JoinStudyGroup(1, 100);

            _mockRepo.Verify(r => r.JoinStudyGroup(1, 100), Times.Once);
            Assert.IsInstanceOf<OkResult>(result);
        }

        [Test]
        public async Task JoinStudyGroup_AlreadyMember_ReturnsBadRequest()
        {
            _mockRepo.Setup(r => r.JoinStudyGroup(1, 100))
                     .ThrowsAsync(new InvalidOperationException(
                         "User is already a member of this study group."));

            var result = await _controller.JoinStudyGroup(1, 100);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task JoinStudyGroup_NonExistentGroup_ReturnsNotFound()
        {
            _mockRepo.Setup(r => r.JoinStudyGroup(999, 100))
                     .ThrowsAsync(new ArgumentException("Study group not found."));

            var result = await _controller.JoinStudyGroup(999, 100);

            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        // ── LEAVE (3 tests) ───────────────────────────────────────────────────

        [Test]
        [Category("Smoke")]
        public async Task LeaveStudyGroup_ValidIds_CallsRepoAndReturnsOk()
        {
            _mockRepo.Setup(r => r.LeaveStudyGroup(1, 100)).Returns(Task.CompletedTask);

            var result = await _controller.LeaveStudyGroup(1, 100);

            _mockRepo.Verify(r => r.LeaveStudyGroup(1, 100), Times.Once);
            Assert.IsInstanceOf<OkResult>(result);
        }

        [Test]
        public async Task LeaveStudyGroup_NotMember_ReturnsNotFound()
        {
            _mockRepo.Setup(r => r.LeaveStudyGroup(1, 200))
                     .ThrowsAsync(new ArgumentException(
                         "User is not a member of this study group."));

            var result = await _controller.LeaveStudyGroup(1, 200);

            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public async Task LeaveStudyGroup_NonExistentGroup_ReturnsNotFound()
        {
            _mockRepo.Setup(r => r.LeaveStudyGroup(999, 100))
                     .ThrowsAsync(new ArgumentException("Study group not found."));

            var result = await _controller.LeaveStudyGroup(999, 100);

            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }
    }
}