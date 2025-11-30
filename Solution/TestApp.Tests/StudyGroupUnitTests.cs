using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace TestApp.Tests
{
    [TestFixture]
    public class StudyGroupUnitTests
    {
        [Test]
        public void StudyGroup_Constructor_ValidParameters_CreatesInstance()
        {
            // Arrange & Act
            var studyGroup = new StudyGroup(1, "Math Study Group", Subject.Math, DateTime.Now, new List<User>());

            // Assert
            Assert.That(studyGroup.StudyGroupId, Is.EqualTo(1));
            Assert.That(studyGroup.Name, Is.EqualTo("Math Study Group"));
            Assert.That(studyGroup.Subject, Is.EqualTo(Subject.Math));
            Assert.That(studyGroup.Users, Is.Not.Null);
            Assert.That(studyGroup.Users.Count, Is.EqualTo(0));
        }

        [Test]
        public void StudyGroup_Constructor_NameTooShort_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => 
                new StudyGroup(1, "Math", Subject.Math, DateTime.Now, new List<User>()));
        }

        [Test]
        public void StudyGroup_Constructor_NameTooLong_ThrowsException()
        {
            // Arrange & Act & Assert
            var longName = new string('A', 31);
            Assert.Throws<ArgumentException>(() => 
                new StudyGroup(1, longName, Subject.Math, DateTime.Now, new List<User>()));
        }

        [Test]
        public void StudyGroup_AddUser_ValidUser_AddsUser()
        {
            // Arrange
            var studyGroup = new StudyGroup(1, "Math Study Group", Subject.Math, DateTime.Now, new List<User>());
            var user = new User(1, "John Doe");

            // Act
            studyGroup.AddUser(user);

            // Assert
            Assert.That(studyGroup.Users.Count, Is.EqualTo(1));
            Assert.That(studyGroup.Users[0].Id, Is.EqualTo(1));
        }

        [Test]
        public void StudyGroup_AddUser_DuplicateUser_DoesNotAddDuplicate()
        {
            // Arrange
            var studyGroup = new StudyGroup(1, "Math Study Group", Subject.Math, DateTime.Now, new List<User>());
            var user = new User(1, "John Doe");

            // Act
            studyGroup.AddUser(user);
            studyGroup.AddUser(user); // Try to add same user again

            // Assert
            Assert.That(studyGroup.Users.Count, Is.EqualTo(1));
        }

        [Test]
        public void StudyGroup_RemoveUser_ExistingUser_RemovesUser()
        {
            // Arrange
            var studyGroup = new StudyGroup(1, "Math Study Group", Subject.Math, DateTime.Now, new List<User>());
            var user = new User(1, "John Doe");
            studyGroup.AddUser(user);

            // Act
            studyGroup.RemoveUser(user);

            // Assert
            Assert.That(studyGroup.Users.Count, Is.EqualTo(0));
        }

        [Test]
        public void StudyGroup_RemoveUser_NonExistingUser_DoesNothing()
        {
            // Arrange
            var studyGroup = new StudyGroup(1, "Math Study Group", Subject.Math, DateTime.Now, new List<User>());
            var user1 = new User(1, "John Doe");
            var user2 = new User(2, "Jane Smith");
            studyGroup.AddUser(user1);

            // Act
            studyGroup.RemoveUser(user2);

            // Assert
            Assert.That(studyGroup.Users.Count, Is.EqualTo(1));
        }

        [Test]
        public void StudyGroup_Constructor_InvalidSubject_ThrowsException()
        {
            // Arrange & Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => 
            {
                var invalidSubject = (Subject)999;
                new StudyGroup(1, "Valid Name Length", invalidSubject, DateTime.Now, new List<User>());
            });
            
            Assert.That(exception.Message, Does.Contain("is not valid"));
        }

        [Test]
        public void StudyGroup_Constructor_NullUsers_InitializesEmptyList()
        {
            // Arrange & Act
            var studyGroup = new StudyGroup(1, "Math Study Group", Subject.Math, DateTime.Now, null);

            // Assert
            Assert.That(studyGroup.Users, Is.Not.Null);
            Assert.That(studyGroup.Users.Count, Is.EqualTo(0));
        }
    }
}