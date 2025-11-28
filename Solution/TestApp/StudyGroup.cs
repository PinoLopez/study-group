using System;
using System.Collections.Generic;
using System.Linq;

namespace TestApp;

public class StudyGroup
{
    private static readonly HashSet<Subject> ValidSubjects = 
        new() { Subject.Math, Subject.Chemistry, Subject.Physics };

    public StudyGroup(int studyGroupId, string name, Subject subject, DateTime createDate, List<User> users)
    {
        // Validation Logic
        if (string.IsNullOrWhiteSpace(name) || name.Length < 5 || name.Length > 30)
        {
            throw new ArgumentException("Name must be between 5 and 30 characters.");
        }
        
        if (!ValidSubjects.Contains(subject))
        {
            throw new ArgumentException($"Subject must be one of: {string.Join(", ", ValidSubjects)}");
        }

        StudyGroupId = studyGroupId;
        Name = name;
        Subject = subject;
        CreateDate = createDate;
        Users = users ?? new List<User>();
    }

    public int StudyGroupId { get; }
    public string Name { get; }
    public Subject Subject { get; }
    public DateTime CreateDate { get; }
    public IReadOnlyList<User> Users { get; private set; }

    public void AddUser(User user)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        
        var newUsers = Users.ToList();
        if (!newUsers.Any(u => u.Id == user.Id))
        {
            newUsers.Add(user);
            Users = newUsers.AsReadOnly();
        }
    }

    public void RemoveUser(User user)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        
        var newUsers = Users.ToList();
        var userToRemove = newUsers.FirstOrDefault(u => u.Id == user.Id);
        if (userToRemove != null)
        {
            newUsers.Remove(userToRemove);
            Users = newUsers.AsReadOnly();
        }
    }

    public StudyGroup WithUsers(List<User> newUsers)
    {
        return new StudyGroup(StudyGroupId, Name, Subject, CreateDate, newUsers);
    }
}

public enum Subject
{
    Math,
    Chemistry,
    Physics
}