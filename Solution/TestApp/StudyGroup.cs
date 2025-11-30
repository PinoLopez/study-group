using System;
using System.Collections.Generic;
using System.Linq;

namespace TestApp
{
    public class StudyGroup
    {
        private const int MinNameLength = 5;
        private const int MaxNameLength = 30;

        public StudyGroup()
        {
            // Parameterless constructor for EF
            Users = new List<User>();
        }

        public StudyGroup(int studyGroupId, string name, Subject subject, DateTime createDate, List<User> users)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Study group name cannot be null or empty.", nameof(name));

            if (name.Length < MinNameLength || name.Length > MaxNameLength)
                throw new ArgumentException($"Study group name must be between {MinNameLength} and {MaxNameLength} characters.", nameof(name));

            // Validate that the subject is a defined enum value
            if (!Enum.IsDefined(typeof(Subject), subject))
                throw new ArgumentException($"Subject '{subject}' is not valid. Valid subjects are: {string.Join(", ", Enum.GetNames(typeof(Subject)))}.", nameof(subject));

            StudyGroupId = studyGroupId;
            Name = name;
            Subject = subject;
            CreateDate = createDate;
            Users = users ?? new List<User>();
        }

        public int StudyGroupId { get; set; }
        public string Name { get; set; } = string.Empty;
        public Subject Subject { get; set; }
        public DateTime CreateDate { get; set; }
        public List<User> Users { get; set; } = new List<User>();

        public void AddUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
                
            // Check if user already exists by Id before adding
            if (!Users.Any(u => u.Id == user.Id))
            {
                Users.Add(user);
            }
        }

        public void RemoveUser(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            
            // Find user by Id instead of reference
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