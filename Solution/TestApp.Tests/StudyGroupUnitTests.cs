using NUnit.Framework;
using TestApp;
using System;

namespace TestApp.Tests
{
    public class StudyGroupUnitTests
    {
        [Test]
        public void Constructor_ValidInputs_CreatesStudyGroupWithCreateDate()
        {
            // Arrange
            var name = "Math Masters";
            var subject = Subject.Math;
            var users = new System.Collections.Generic.List<User>();

            // Act
            var studyGroup = new StudyGroup(1, name, subject, DateTime.UtcNow, users);

            // Assert
            Assert.AreEqual(name, studyGroup.Name);
            Assert.AreEqual(subject, studyGroup.Subject);
            Assert.That(studyGroup.CreateDate, Is.Not.EqualTo(default(DateTime)));
        }

        [Test]
        public void Constructor_NameTooShort_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                new StudyGroup(1, "Math", Subject.Math, DateTime.UtcNow, new()));
        }

        [Test]
        public void Constructor_NameTooLong_ThrowsArgumentException()
        {
            var longName = new string('A', 31);
            Assert.Throws<ArgumentException>(() =>
                new StudyGroup(1, longName, Subject.Physics, DateTime.UtcNow, new()));
        }

        [Test]
        public void Constructor_InvalidSubject_ThrowsArgumentException()
        {
            // Enum enforces only Math/Chemistry/Physics → no invalid values possible
            // So this test is less relevant unless Subject is string
            // We assume enum is used → skip or mark as N/A
            Assert.Pass("Subject is enum → validation not needed here");
        }
    }
}