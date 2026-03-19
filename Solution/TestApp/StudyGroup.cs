using System;
using System.Collections.Generic;
using System.Linq;

namespace TestApp
{
    public class StudyGroup
    {
        public int StudyGroupId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public Subject Subject { get; private set; }
        public DateTime CreateDate { get; private set; }
        public List<User> Users { get; private set; } = new();

        private StudyGroup() { } // EF Core

        public StudyGroup(string name, Subject subject)
        {
            ValidateName(name);
            ValidateSubject(subject);
            Name = name;
            Subject = subject;
            CreateDate = DateTime.UtcNow;
            StudyGroupId = 0; // will be set by DB
        }

        public StudyGroup(int studyGroupId, string name, Subject subject, DateTime createDate, List<User> users)
        {
            StudyGroupId = studyGroupId;
            Name = name;
            Subject = subject;
            CreateDate = createDate;
            Users = users ?? new List<User>();
        }

        private void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length < 5 || name.Length > 30)
                throw new ArgumentException("Name must be 5-30 characters.");
        }

        private void ValidateSubject(Subject subject)
        {
            if (!Enum.IsDefined(typeof(Subject), subject))
                throw new ArgumentException("Invalid subject. Only Math, Chemistry, Physics allowed.");
        }

        public void AddUser(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (Users.Any(u => u.Id == user.Id))
                throw new InvalidOperationException("User already in group.");
            Users.Add(user);
        }

        public void RemoveUser(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (!Users.Remove(user))
                throw new InvalidOperationException("User not in group.");
        }
    }

    public enum Subject { Math, Chemistry, Physics }
}