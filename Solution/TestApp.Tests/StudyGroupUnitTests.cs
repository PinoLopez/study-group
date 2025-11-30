using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace TestApp.Tests
{
    [TestFixture]
    public class StudyGroupUnitTests
    {
        [Test(Description = "AC 1: Verifies valid StudyGroup creation with correct property assignment.")]
        public void StudyGroup_Constructor_ValidParameters_CreatesInstance()
        {
            TestContext.WriteLine("Creating StudyGroup with valid, non-null parameters...");

            // AC 1c: Checks for CreateDate recording
            var creationTime = DateTime.UtcNow.Date;
            var studyGroup = new StudyGroup(1, "Math Study Group", Subject.Math, creationTime, new List<User>());

            TestContext.WriteLine($"Created StudyGroup: ID={studyGroup.StudyGroupId}, Name={studyGroup.Name}, Subject={studyGroup.Subject}, Users={studyGroup.Users.Count}");

            Assert.That(studyGroup.StudyGroupId, Is.EqualTo(1));
            Assert.That(studyGroup.Name, Is.EqualTo("Math Study Group"));
            Assert.That(studyGroup.Subject, Is.EqualTo(Subject.Math));
            Assert.That(studyGroup.CreateDate.Date, Is.EqualTo(creationTime)); // AC 1c check
            Assert.That(studyGroup.Users, Is.Not.Null);
            Assert.That(studyGroup.Users.Count, Is.EqualTo(0));
        }

        [Test(Description = "AC 1a: Ensures constructor throws ArgumentException when name is too short (less than 5 chars).")]
        public void StudyGroup_Constructor_NameTooShort_ThrowsException()
        {
            TestContext.WriteLine("Testing constructor with name too short (\"Math\")...");

            Assert.Throws<ArgumentException>(() =>
                new StudyGroup(1, "Math", Subject.Math, DateTime.Now, new List<User>())
            );

            TestContext.WriteLine("Exception was correctly thrown for short name.");
        }

        [Test(Description = "AC 1a: Ensures constructor throws ArgumentException when name is too long (over 30 chars).")]
        public void StudyGroup_Constructor_NameTooLong_ThrowsException()
        {
            var longName = new string('A', 31);
            TestContext.WriteLine($"Testing constructor with name length {longName.Length}...");

            Assert.Throws<ArgumentException>(() =>
                new StudyGroup(1, longName, Subject.Math, DateTime.Now, new List<User>())
            );

            TestContext.WriteLine("Exception was correctly thrown for long name.");
        }

        [Test(Description = "AC 2: Adds a valid user to the StudyGroup.")]
        public void StudyGroup_AddUser_ValidUser_AddsUser()
        {
            var studyGroup = new StudyGroup(1, "Math Study Group", Subject.Math, DateTime.Now, new List<User>());
            var user = new User(1, "Miguel Perez");

            TestContext.WriteLine("Adding user Miguel Perez (ID=1) to study group...");

            studyGroup.AddUser(user);

            TestContext.WriteLine($"StudyGroup now contains {studyGroup.Users.Count} user(s).");

            Assert.That(studyGroup.Users.Count, Is.EqualTo(1));
            Assert.That(studyGroup.Users[0].Id, Is.EqualTo(1));
        }

        [Test(Description = "AC 2: Ensures duplicate users (same ID) are not added to StudyGroup.")]
        public void StudyGroup_AddUser_DuplicateUser_DoesNotAddDuplicate()
        {
            var studyGroup = new StudyGroup(1, "Math Study Group", Subject.Math, DateTime.Now, new List<User>());
            var user = new User(1, "Miguel Perez");

            TestContext.WriteLine("Adding same user twice to study group...");

            studyGroup.AddUser(user);
            studyGroup.AddUser(user);

            TestContext.WriteLine($"After attempting duplicate add: User count = {studyGroup.Users.Count}");

            Assert.That(studyGroup.Users.Count, Is.EqualTo(1));
        }

        [Test(Description = "AC 4: Removes an existing user from StudyGroup.")]
        public void StudyGroup_RemoveUser_ExistingUser_RemovesUser()
        {
            var studyGroup = new StudyGroup(1, "Math Study Group", Subject.Math, DateTime.Now, new List<User>());
            var user = new User(1, "Miguel Perez");
            studyGroup.AddUser(user);

            TestContext.WriteLine("Removing existing user Miguel Perez...");

            studyGroup.RemoveUser(user);

            TestContext.WriteLine($"After removal: User count = {studyGroup.Users.Count}");

            Assert.That(studyGroup.Users.Count, Is.EqualTo(0));
        }

        [Test(Description = "AC 4: Ensures removing a non-existing user does nothing.")]
        public void StudyGroup_RemoveUser_NonExistingUser_DoesNothing()
        {
            var studyGroup = new StudyGroup(1, "Math Study Group", Subject.Math, DateTime.Now, new List<User>());
            var user1 = new User(1, "Miguel Perez");
            var user2 = new User(2, "Manuel Pino");
            studyGroup.AddUser(user1);

            TestContext.WriteLine("Attempting to remove non-existing user Manuel Pino...");

            studyGroup.RemoveUser(user2);

            TestContext.WriteLine($"After removal attempt: User count = {studyGroup.Users.Count}");

            Assert.That(studyGroup.Users.Count, Is.EqualTo(1));
        }

        [Test(Description = "AC 1b: Constructor rejects invalid Subject enum values.")]
        public void StudyGroup_Constructor_InvalidSubject_ThrowsException()
        {
            TestContext.WriteLine("Testing constructor with invalid subject enum value...");

            var invalidSubject = (Subject)999;

            var exception = Assert.Throws<ArgumentException>(() =>
                new StudyGroup(1, "Valid Name Length", invalidSubject, DateTime.Now, new List<User>())
            );

            TestContext.WriteLine($"Exception message: {exception.Message}");

            Assert.That(exception.Message, Does.Contain("is not valid"));
        }

        [Test(Description = "Regression: Ensures a null user list initializes an empty list.")]
        public void StudyGroup_Constructor_NullUsers_InitializesEmptyList()
        {
            TestContext.WriteLine("Testing constructor with null user list...");

            var studyGroup = new StudyGroup(1, "Math Study Group", Subject.Math, DateTime.Now, null);

            TestContext.WriteLine($"Users list initialized. Count = {studyGroup.Users.Count}");

            Assert.That(studyGroup.Users, Is.Not.Null);
            Assert.That(studyGroup.Users.Count, Is.EqualTo(0));
        }
    }
}
