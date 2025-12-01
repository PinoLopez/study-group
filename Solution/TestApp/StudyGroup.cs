using System;
using System.Collections.Generic;
using System.Linq;

namespace TestApp
{
    public class StudyGroup
    {
        private const int MinNameLength = 5;
        private const int MaxNameLength = 30;

        // Parameterless constructor for EF Core
        protected StudyGroup() 
        {
            Users = new List<User>();
        }

        public StudyGroup(int studyGroupId, string name, Subject subject, DateTime createDate, List<User> users)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Study group name cannot be null or empty.", nameof(name));
            if (name.Length < MinNameLength || name.Length > MaxNameLength)
                throw new ArgumentException($"Study group name must be between {MinNameLength} and {MaxNameLength} characters.", nameof(name));
            if (!Enum.IsDefined(typeof(Subject), subject))
                throw new ArgumentException($"Subject '{subject}' is not valid. Valid subjects are: {string.Join(", ", Enum.GetNames(typeof(Subject)))}.", nameof(subject));

            StudyGroupId = studyGroupId;
            Name = name;
            Subject = subject;
            CreateDate = createDate;
            Users = users ?? new List<User>();
        }

        public int StudyGroupId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public Subject Subject { get; private set; }
        public DateTime CreateDate { get; private set; }
        public List<User> Users { get; private set; } = new List<User>();

        public void AddUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (!Users.Any(u => u.Id == user.Id))
            {
                Users.Add(user);
            }
        }

        public void RemoveUser(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            var userToRemove = Users.FirstOrDefault(u => u.Id == user.Id);
            if (userToRemove != null)
            {
                Users.Remove(userToRemove);
            }
        }
    }

    public enum Subject
    {
        Math,
        Chemistry,
        Physics
    }
}