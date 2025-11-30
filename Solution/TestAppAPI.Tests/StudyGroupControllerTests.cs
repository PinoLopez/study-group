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
        // Null-forgiving operator is used here since these are initialized in [SetUp]
        private Mock<IStudyGroupRepository> _mockRepo = null!;
        private StudyGroupController _controller = null!;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IStudyGroupRepository>();
            _controller = new StudyGroupController(_mockRepo.Object);
        }

        #region Create Study Group

        [Test(Description = "AC 1: Given valid StudyGroup, returns 200 Ok on creation.")]
        public async Task GivenValidStudyGroup_WhenCreating_ThenReturnsOk()
        {
            TestContext.WriteLine("Test AC 1: Attempting to create a valid Study Group (Math Study Club).");
            var group = new StudyGroup(0, "Math Study Club", Subject.Math, DateTime.UtcNow, new List<User>());
            _mockRepo.Setup(r => r.CreateStudyGroup(group)).Returns(Task.CompletedTask);

            var result = await _controller.CreateStudyGroup(group);

            Assert.That(result, Is.InstanceOf<OkResult>());
            TestContext.WriteLine("Result: Controller returned 200 Ok, confirming successful creation.");
        }

        [Test(Description = "AC 1: Given duplicate subject, returns 400 Bad Request.")]
        public async Task GivenStudyGroupWithDuplicateSubject_WhenCreating_ThenReturnsBadRequest()
        {
            TestContext.WriteLine("Test AC 1: Attempting to create a Study Group with a duplicate subject (Chemistry).");
            var group = new StudyGroup(0, "Chemistry Study Group", Subject.Chemistry, DateTime.UtcNow, new List<User>());
            _mockRepo.Setup(r => r.CreateStudyGroup(group))
                     .ThrowsAsync(new InvalidOperationException("A study group for subject 'Chemistry' already exists."));

            var result = await _controller.CreateStudyGroup(group);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badResult = (BadRequestObjectResult)result!;
            Assert.That(badResult.Value!.ToString(), Does.Contain("already exists"));
            TestContext.WriteLine($"Result: Controller returned 400 Bad Request with message: '{badResult.Value}' (Duplicate prevented).");
        }

        [Test(Description = "AC 1a: Given invalid name length, returns 400 Bad Request.")]
        public async Task GivenStudyGroupNameTooShort_WhenCreating_ThenReturnsBadRequest()
        {
            TestContext.WriteLine("Test AC 1a: Attempting to create a Study Group with a name too short ('Short').");
            var group = new StudyGroup(0, "Short", Subject.Math, DateTime.UtcNow, new List<User>());

            _mockRepo.Setup(r => r.CreateStudyGroup(It.IsAny<StudyGroup>()))
                     .ThrowsAsync(new ArgumentException("Study group name must be between 5 and 30 characters.", "name"));

            var result = await _controller.CreateStudyGroup(group);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badResult = (BadRequestObjectResult)result!;
            Assert.That(badResult.Value!.ToString(), Does.Contain("between 5 and 30"));
            TestContext.WriteLine("Result: Controller returned 400 Bad Request. Validation error caught by the domain model.");
        }

        [Test(Description = "General: Given null request body, returns 400 Bad Request.")]
        public async Task GivenNullStudyGroup_WhenCreating_ThenReturnsBadRequest()
        {
            TestContext.WriteLine("Test General: Attempting to create a Study Group with a null request body.");
            var result = await _controller.CreateStudyGroup(null!);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            TestContext.WriteLine("Result: Controller returned 400 Bad Request. Null input successfully handled.");
        }

        #endregion

        #region Get Study Groups

        [Test(Description = "AC 3: Given no sort parameter, returns groups sorted by CreateDate DESC (newest first).")]
        public async Task GivenNoSort_WhenGettingAllGroups_ThenReturnsGroupsSortedByCreationDateDescending()
        {
            TestContext.WriteLine("Test AC 3: Retrieving all groups with default sorting (DESC by CreateDate).");
            var groups = new List<StudyGroup>
            {
                new StudyGroup(2, "Advanced Math Studies", Subject.Math, DateTime.UtcNow, new List<User>()),
                new StudyGroup(1, "Mathematics Study Group", Subject.Math, DateTime.UtcNow.AddDays(-1), new List<User>())
            };
            _mockRepo.Setup(r => r.GetStudyGroups("desc")).ReturnsAsync(groups);

            var result = await _controller.GetStudyGroups();

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            var returned = okResult!.Value as List<StudyGroup>;
            Assert.That(returned![0].Name, Is.EqualTo("Advanced Math Studies"));
            TestContext.WriteLine($"Result: Controller returned 200 Ok. First group is '{returned[0].Name}', confirming DESC order.");
        }

        [Test(Description = "AC 3b: Given 'asc' sort parameter, returns groups sorted by CreateDate ASC (oldest first).")]
        public async Task GivenSortAscending_WhenGettingAllGroups_ThenReturnsGroupsSortedByCreationDateAscending()
        {
            TestContext.WriteLine("Test AC 3b: Retrieving all groups sorted by CreateDate ASC.");
            var groups = new List<StudyGroup>
            {
                new StudyGroup(1, "Physics Fundamentals", Subject.Physics, DateTime.UtcNow.AddDays(-2), new List<User>()),
                new StudyGroup(2, "Modern Physics Group", Subject.Physics, DateTime.UtcNow, new List<User>())
            };
            _mockRepo.Setup(r => r.GetStudyGroups("asc")).ReturnsAsync(groups);

            var result = await _controller.GetStudyGroups(sort: "asc");

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            var returned = okResult!.Value as List<StudyGroup>;
            Assert.That(returned![0].Name, Is.EqualTo("Physics Fundamentals"));
            TestContext.WriteLine($"Result: Controller returned 200 Ok. First group is '{returned[0].Name}', confirming ASC order.");
        }

        #endregion

        #region Search Study Groups

        [Test(Description = "AC 3a: Given valid subject string, returns matching study groups.")]
        public async Task GivenValidSubject_WhenSearching_ThenReturnsMatchingStudyGroups()
        {
            TestContext.WriteLine("Test AC 3a: Searching for groups by valid subject 'Physics'.");
            var groups = new List<StudyGroup> { new StudyGroup(1, "Quantum Physics Group", Subject.Physics, DateTime.UtcNow, new List<User>()) };
            _mockRepo.Setup(r => r.SearchStudyGroups("Physics", "desc")).ReturnsAsync(groups);

            var result = await _controller.SearchStudyGroups("Physics");

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            var returned = okResult!.Value as List<StudyGroup>;
            Assert.That(returned![0].Subject, Is.EqualTo(Subject.Physics));
            TestContext.WriteLine("Result: Controller returned 200 Ok with groups filtered by Physics.");
        }

        [Test(Description = "AC 3a: Given invalid subject, returns 400 Bad Request.")]
        public async Task GivenInvalidSubject_WhenSearching_ThenReturnsBadRequest()
        {
            TestContext.WriteLine("Test AC 3a: Searching for groups by invalid subject 'Biology'.");
            _mockRepo.Setup(r => r.SearchStudyGroups("Biology", "desc"))
                     .ThrowsAsync(new ArgumentException("Invalid subject."));

            var result = await _controller.SearchStudyGroups("Biology");

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            TestContext.WriteLine("Result: Controller returned 400 Bad Request. Invalid subject rejected.");
        }

        [Test(Description = "AC 3a & 3b: Given valid subject and sort ASC, returns filtered and sorted groups.")]
        public async Task GivenValidSubjectAndSortAscending_WhenSearching_ThenReturnsSortedGroups()
        {
            TestContext.WriteLine("Test AC 3a & 3b: Searching by subject 'Physics' and sorting ASC.");
            var old = new StudyGroup(1, "Classical Physics Group", Subject.Physics, DateTime.UtcNow.AddDays(-1), new List<User>());
            var recent = new StudyGroup(2, "Modern Physics Studies", Subject.Physics, DateTime.UtcNow, new List<User>());

            _mockRepo.Setup(r => r.SearchStudyGroups("Physics", "asc"))
                     .ReturnsAsync(new List<StudyGroup> { old, recent });

            var result = await _controller.SearchStudyGroups("Physics", sort: "asc");

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            var returned = okResult!.Value as List<StudyGroup>;
            Assert.That(returned![0].Name, Is.EqualTo("Classical Physics Group"));
            TestContext.WriteLine("Result: Controller returned 200 Ok with filtered and ASC sorted groups.");
        }

        #endregion

        #region Join Study Group

        [Test(Description = "AC 2: Given valid IDs, returns 200 Ok on successful join.")]
        public async Task GivenValidStudyGroupIdAndUserId_WhenJoining_ThenReturnsOk()
        {
            TestContext.WriteLine("Test AC 2: Attempting to join StudyGroup 5 with User 101.");
            _mockRepo.Setup(r => r.JoinStudyGroup(5, 101)).Returns(Task.CompletedTask);

            var result = await _controller.JoinStudyGroup(5, 101);

            Assert.That(result, Is.InstanceOf<OkResult>());
            TestContext.WriteLine("Result: Controller returned 200 Ok, confirming successful join.");
        }

        [Test(Description = "AC 2: Given non-existent StudyGroup, returns 404 Not Found.")]
        public async Task GivenNonExistentStudyGroup_WhenJoining_ThenReturnsNotFound()
        {
            TestContext.WriteLine("Test AC 2: Attempting to join a non-existent StudyGroup (ID 999).");
            _mockRepo.Setup(r => r.JoinStudyGroup(999, 101))
                     .ThrowsAsync(new ArgumentException("Study group not found."));

            var result = await _controller.JoinStudyGroup(999, 101);

            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            TestContext.WriteLine("Result: Controller returned 404 Not Found, as expected.");
        }

        [Test(Description = "AC 2: Given user already in group, returns 400 Bad Request.")]
        public async Task GivenUserAlreadyInGroup_WhenJoining_ThenReturnsBadRequest()
        {
            TestContext.WriteLine("Test AC 2: Attempting to join a StudyGroup when the user is already a member.");
            _mockRepo.Setup(r => r.JoinStudyGroup(5, 101))
                     .ThrowsAsync(new InvalidOperationException("User is already a member of this study group."));

            var result = await _controller.JoinStudyGroup(5, 101);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            TestContext.WriteLine("Result: Controller returned 400 Bad Request. Duplicate join prevented.");
        }

        #endregion

        #region Leave Study Group

        [Test(Description = "AC 4: Given valid IDs, returns 200 Ok on successful leave.")]
        public async Task GivenValidStudyGroupIdAndUserId_WhenLeaving_ThenReturnsOk()
        {
            TestContext.WriteLine("Test AC 4: Attempting to leave StudyGroup 5 with User 101.");
            _mockRepo.Setup(r => r.LeaveStudyGroup(5, 101)).Returns(Task.CompletedTask);

            var result = await _controller.LeaveStudyGroup(5, 101);

            Assert.That(result, Is.InstanceOf<OkResult>());
            TestContext.WriteLine("Result: Controller returned 200 Ok, confirming successful leave.");
        }

        [Test(Description = "AC 4: Given non-existent StudyGroup, returns 404 Not Found.")]
        public async Task GivenNonExistentStudyGroup_WhenLeaving_ThenReturnsNotFound()
        {
            TestContext.WriteLine("Test AC 4: Attempting to leave a non-existent StudyGroup (ID 999).");
            _mockRepo.Setup(r => r.LeaveStudyGroup(999, 101))
                     .ThrowsAsync(new ArgumentException("Study group not found."));

            var result = await _controller.LeaveStudyGroup(999, 101);

            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            TestContext.WriteLine("Result: Controller returned 404 Not Found, as expected.");
        }

        [Test(Description = "AC 4: Given user not in group, returns 404 Not Found.")]
        public async Task GivenUserNotInGroup_WhenLeaving_ThenReturnsNotFound()
        {
            TestContext.WriteLine("Test AC 4: Attempting to leave a StudyGroup when the user is not a member (User ID 999).");
            _mockRepo.Setup(r => r.LeaveStudyGroup(5, 999))
                     .ThrowsAsync(new ArgumentException("User is not a member of this study group."));

            var result = await _controller.LeaveStudyGroup(5, 999);

            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            TestContext.WriteLine("Result: Controller returned 404 Not Found, as the user was not a member.");
        }

        #endregion
    }
}
