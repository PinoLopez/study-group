using System;
using System.Collections.Generic;

namespace TestApp
{
    public class StudyGroup
    {
        public int StudyGroupId { get; set; }
        public string Name { get; set; } = string.Empty;
        public Subject Subject { get; set; }
        public DateTime CreateDate { get; set; }
        public List<User> Users { get; private set; } = new List<User>();

        // Validation in constructor (per PDF note)
        public StudyGroup() { }  // For EF

        public StudyGroup(int studyGroupId, string name, Subject subject, DateTime createDate, List<User>? users = null)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length < 5 || name.Length > 30)
                throw new ArgumentException("Name must be between 5 and 30 characters.", nameof(name));

            if (!Enum.IsDefined(typeof(Subject), subject))
                throw new ArgumentException("Invalid subject. Must be Math, Chemistry, or Physics.", nameof(subject));

            StudyGroupId = studyGroupId;
            Name = name;
            Subject = subject;
            CreateDate = createDate;
            Users = users ?? new List<User>();
        }

        public void AddUser(User user)
        {
            if (user != null && !Users.Contains(user))
                Users.Add(user);
        }

        public void RemoveUser(User user)
        {
            Users.Remove(user);
        }
    }

    public enum Subject
    {
        Math,
        Chemistry,
        Physics
    }
}