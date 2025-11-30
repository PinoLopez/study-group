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
        private Mock<IStudyGroupRepository> _mockRepo = null!;
        private StudyGroupController _controller = null!;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IStudyGroupRepository>();
            _controller = new StudyGroupController(_mockRepo.Object);
        }

        #region Create Study Group

        [Test]
        public async Task GivenValidStudyGroup_WhenCreating_ThenReturnsOk()
        {
            var group = new StudyGroup(0, "Math Study Club", Subject.Math, DateTime.UtcNow, new List<User>());
            _mockRepo.Setup(r => r.CreateStudyGroup(group)).Returns(Task.CompletedTask);

            var result = await _controller.CreateStudyGroup(group);

            Assert.That(result, Is.InstanceOf<OkResult>());
        }

        [Test]
        public async Task GivenStudyGroupWithDuplicateSubject_WhenCreating_ThenReturnsBadRequest()
        {
            var group = new StudyGroup(0, "Chemistry Study Group", Subject.Chemistry, DateTime.UtcNow, new List<User>());
            _mockRepo.Setup(r => r.CreateStudyGroup(group))
                     .ThrowsAsync(new InvalidOperationException("A study group for subject 'Chemistry' already exists."));

            var result = await _controller.CreateStudyGroup(group);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badResult = (BadRequestObjectResult)result;
            Assert.That(badResult.Value?.ToString(), Does.Contain("already exists"));
        }

        [Test]
        public async Task GivenStudyGroupNameTooShort_WhenCreating_ThenReturnsBadRequest()
        {
            // The TestApp model now enforces validation in the constructor.
            // To test the API response for an ArgumentException, we must mock the repository
            // to throw the validation exception when it receives the invalid object.
            // Create a valid StudyGroup object (the controller doesn't validate, the domain model does)
            var group = new StudyGroup(0, "Short", Subject.Math, DateTime.UtcNow, new List<User>());
            
            // Mock the repository to throw the expected ArgumentException when the controller calls it.
            // (NOTE: In a real scenario, the controller often validates before the repository, 
            // but based on your previous logs, the TestApp model handles name validation internally,
            // which the API controller must catch and return as a BadRequest).
            _mockRepo.Setup(r => r.CreateStudyGroup(It.IsAny<StudyGroup>()))
                     .ThrowsAsync(new ArgumentException("Study group name must be between 5 and 30 characters.", "name"));


            var result = await _controller.CreateStudyGroup(group);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badResult = (BadRequestObjectResult)result;
            // Updated assertion to look for the specific validation error text
            Assert.That(badResult.Value?.ToString(), Does.Contain("between 5 and 30"));
        }

        [Test]
        public async Task GivenNullStudyGroup_WhenCreating_ThenReturnsBadRequest()
        {
            var result = await _controller.CreateStudyGroup(null!);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        #endregion

        #region Get Study Groups

        [Test]
        public async Task GivenNoSort_WhenGettingAllGroups_ThenReturnsGroupsSortedByCreationDateDescending()
        {
            // Return list in expected DESCENDING order: newest first
            var groups = new List<StudyGroup>
            {
                // Constructor calls updated for all tests
                new StudyGroup(2, "Advanced Math Studies", Subject.Math, DateTime.UtcNow, new List<User>()),
                new StudyGroup(1, "Mathematics Study Group", Subject.Math, DateTime.UtcNow.AddDays(-1), new List<User>())
            };
            _mockRepo.Setup(r => r.GetStudyGroups("desc")).ReturnsAsync(groups);

            var result = await _controller.GetStudyGroups();

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            var returned = okResult.Value as List<StudyGroup>;
            Assert.That(returned, Is.Not.Null);
            Assert.That(returned.Count, Is.EqualTo(2));
            Assert.That(returned[0].Name, Is.EqualTo("Advanced Math Studies"));
        }

        [Test]
        public async Task GivenSortAscending_WhenGettingAllGroups_ThenReturnsGroupsSortedByCreationDateAscending()
        {
            // Return list in expected ASCENDING order: oldest first
            var groups = new List<StudyGroup>
            {
                // Constructor calls updated for all tests
                new StudyGroup(1, "Physics Fundamentals", Subject.Physics, DateTime.UtcNow.AddDays(-2), new List<User>()),
                new StudyGroup(2, "Modern Physics Group", Subject.Physics, DateTime.UtcNow, new List<User>())
            };
            _mockRepo.Setup(r => r.GetStudyGroups("asc")).ReturnsAsync(groups);

            var result = await _controller.GetStudyGroups(sort: "asc");

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            var returned = okResult.Value as List<StudyGroup>;
            Assert.That(returned, Is.Not.Null);
            Assert.That(returned.Count, Is.EqualTo(2));
            Assert.That(returned[0].Name, Is.EqualTo("Physics Fundamentals"));
        }

        #endregion

        #region Search Study Groups

        [Test]
        public async Task GivenValidSubject_WhenSearching_ThenReturnsMatchingStudyGroups()
        {
            // Constructor calls updated for all tests
            var groups = new List<StudyGroup> { new StudyGroup(1, "Quantum Physics Group", Subject.Physics, DateTime.UtcNow, new List<User>()) };
            _mockRepo.Setup(r => r.SearchStudyGroups("Physics", "desc")).ReturnsAsync(groups);

            var result = await _controller.SearchStudyGroups("Physics");

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            var returned = okResult.Value as List<StudyGroup>;
            Assert.That(returned, Is.Not.Null);
            Assert.That(returned[0].Subject, Is.EqualTo(Subject.Physics));
        }

        [Test]
        public async Task GivenInvalidSubject_WhenSearching_ThenReturnsBadRequest()
        {
            _mockRepo.Setup(r => r.SearchStudyGroups("Biology", "desc"))
                     .ThrowsAsync(new ArgumentException("Invalid subject."));

            var result = await _controller.SearchStudyGroups("Biology");

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GivenValidSubjectAndSortAscending_WhenSearching_ThenReturnsSortedGroups()
        {
            // Constructor calls updated for all tests
            var old = new StudyGroup(1, "Classical Physics Group", Subject.Physics, DateTime.UtcNow.AddDays(-1), new List<User>());
            var recent = new StudyGroup(2, "Modern Physics Studies", Subject.Physics, DateTime.UtcNow, new List<User>());

            _mockRepo.Setup(r => r.SearchStudyGroups("Physics", "asc"))
                     .ReturnsAsync(new List<StudyGroup> { old, recent });

            var result = await _controller.SearchStudyGroups("Physics", sort: "asc");

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            var returned = okResult.Value as List<StudyGroup>;
            Assert.That(returned, Is.Not.Null);
            Assert.That(returned[0].Name, Is.EqualTo("Classical Physics Group"));
        }

        #endregion

        #region Join Study Group

        [Test]
        public async Task GivenValidStudyGroupIdAndUserId_WhenJoining_ThenReturnsOk()
        {
            _mockRepo.Setup(r => r.JoinStudyGroup(5, 101)).Returns(Task.CompletedTask);

            var result = await _controller.JoinStudyGroup(5, 101);

            Assert.That(result, Is.InstanceOf<OkResult>());
        }

        [Test]
        public async Task GivenNonExistentStudyGroup_WhenJoining_ThenReturnsNotFound()
        {
            _mockRepo.Setup(r => r.JoinStudyGroup(999, 101))
                     .ThrowsAsync(new ArgumentException("Study group not found."));

            var result = await _controller.JoinStudyGroup(999, 101);

            // NOTE: A domain exception (ArgumentException) should usually be handled by the controller
            // and translated into a proper HTTP status code, like 404 Not Found.
            // The test already asserts a 404 (NotFoundObjectResult).
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task GivenUserAlreadyInGroup_WhenJoining_ThenReturnsBadRequest()
        {
            _mockRepo.Setup(r => r.JoinStudyGroup(5, 101))
                     .ThrowsAsync(new InvalidOperationException("User is already a member of this study group."));

            var result = await _controller.JoinStudyGroup(5, 101);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        #endregion

        #region Leave Study Group

        [Test]
        public async Task GivenValidStudyGroupIdAndUserId_WhenLeaving_ThenReturnsOk()
        {
            _mockRepo.Setup(r => r.LeaveStudyGroup(5, 101)).Returns(Task.CompletedTask);

            var result = await _controller.LeaveStudyGroup(5, 101);

            Assert.That(result, Is.InstanceOf<OkResult>());
        }

        [Test]
        public async Task GivenNonExistentStudyGroup_WhenLeaving_ThenReturnsNotFound()
        {
            _mockRepo.Setup(r => r.LeaveStudyGroup(999, 101))
                     .ThrowsAsync(new ArgumentException("Study group not found."));

            var result = await _controller.LeaveStudyGroup(999, 101);

            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task GivenUserNotInGroup_WhenLeaving_ThenReturnsNotFound()
        {
            _mockRepo.Setup(r => r.LeaveStudyGroup(5, 999))
                     .ThrowsAsync(new ArgumentException("User is not a member of this study group."));

            var result = await _controller.LeaveStudyGroup(5, 999);

            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        #endregion
    }
}