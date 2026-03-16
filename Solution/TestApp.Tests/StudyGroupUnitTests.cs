using NUnit.Framework;
using System;
using TestApp;

namespace TestApp.Tests
{
    [TestFixture]
    public class StudyGroupUnitTests
    {
        // Existing 9 tests (inferred and implemented based on PDF description: constructor validations (name, subject, date), add/remove users, null handling)

        [Test]
        public void Constructor_ValidNameAndSubject_SetsPropertiesCorrectly()
        {
            var group = new StudyGroup("Valid Group", Subject.Math);
            Assert.AreEqual("Valid Group", group.Name);
            Assert.AreEqual(Subject.Math, group.Subject);
        }

        [Test]
        public void Constructor_InvalidNameNull_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => new StudyGroup(null, Subject.Chemistry));
        }

        [Test]
        public void Constructor_InvalidNameEmpty_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => new StudyGroup("", Subject.Physics));
        }

        [Test]
        public void Constructor_InvalidSubject_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => new StudyGroup("Group", (Subject)999));
        }

        [Test]
        public void Constructor_SetsCreationDateAutomatically()
        {
            var group = new StudyGroup("Group", Subject.Math);
            Assert.True(group.CreateDate > DateTime.UtcNow.AddMinutes(-1));
            Assert.True(group.CreateDate <= DateTime.UtcNow);
        }

        [Test]
        public void AddUser_ValidUser_AddsToList()
        {
            var group = new StudyGroup("Group", Subject.Math);
            var user = new User(1, "Test User");
            group.AddUser(user);
            Assert.Contains(user, group.Users);
        }

        [Test]
        public void AddUser_NullUser_ThrowsException()
        {
            var group = new StudyGroup("Group", Subject.Math);
            Assert.Throws<ArgumentNullException>(() => group.AddUser(null));
        }

        [Test]
        public void RemoveUser_ExistingUser_RemovesFromList()
        {
            var group = new StudyGroup("Group", Subject.Math);
            var user = new User(1, "Test User");
            group.AddUser(user);
            group.RemoveUser(user);
            Assert.IsFalse(group.Users.Contains(user));
        }

        [Test]
        public void RemoveUser_NullUser_ThrowsException()
        {
            var group = new StudyGroup("Group", Subject.Math);
            Assert.Throws<ArgumentNullException>(() => group.RemoveUser(null));
        }

        // New tests added (from the test cases I described)

        [Test]
        [Category("Smoke")]
        public void CreateStudyGroup_ValidNameAndSubject_Succeeds()
        {
            var group = new StudyGroup("Valid Group", Subject.Math);
            Assert.AreEqual("Valid Group", group.Name);
            Assert.AreEqual(Subject.Math, group.Subject);
            Assert.True(group.CreateDate > DateTime.UtcNow.AddMinutes(-1));
        }

       [Test]
         public void CreateStudyGroup_InvalidNameTooShort_ThrowsException()
         {
          Assert.Throws<ArgumentException>(() => new StudyGroup("Math", Subject.Chemistry));
         }

        [Test]
        public void CreateStudyGroup_InvalidNameTooLong_ThrowsException()
        {
            var longName = new string('A', 31);
            Assert.Throws<ArgumentException>(() => new StudyGroup(longName, Subject.Physics));
        }

        [Test]
        public void AddUser_DuplicateUser_ThrowsException()
        {
            var group = new StudyGroup("Group", Subject.Math);
            var user = new User(1, "Test User");
            group.AddUser(user);
            Assert.Throws<InvalidOperationException>(() => group.AddUser(user));
        }

        [Test]
        public void RemoveUser_UserNotInGroup_ThrowsException()
        {
            var group = new StudyGroup("Group", Subject.Math);
            var user = new User(1, "Test User");
            Assert.Throws<InvalidOperationException>(() => group.RemoveUser(user));
        }
    }
}