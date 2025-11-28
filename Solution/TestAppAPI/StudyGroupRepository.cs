using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestApp;

namespace TestAppAPI
{
    public class StudyGroupRepository : IStudyGroupRepository
    {
        // In-memory storage simulating DB
        private readonly ConcurrentDictionary<int, StudyGroup> _studyGroups = new();
        private int _nextId = 1;

        public async Task CreateStudyGroup(StudyGroup studyGroup)
        {
            // Only one Study Group for a single Subject
            if (_studyGroups.Values.Any(sg => sg.Subject == studyGroup.Subject))
            {
                throw new InvalidOperationException($"A study group for subject '{studyGroup.Subject}' already exists.");
            }

            // simulating generating an ID.
            var newGroup = new StudyGroup(
                studyGroupId: _nextId++,
                name: studyGroup.Name,
                subject: studyGroup.Subject,
                createDate: DateTime.UtcNow,
                users: new List<User>()
            );

            _studyGroups[newGroup.StudyGroupId] = newGroup;
            await Task.CompletedTask;
        }

        public async Task<IEnumerable<StudyGroup>> GetStudyGroups()
        {
            return await Task.FromResult(_studyGroups.Values.OrderByDescending(x => x.CreateDate).ToList());
        }

        public async Task<IEnumerable<StudyGroup>> SearchStudyGroups(string subject)
        {
            if (string.IsNullOrWhiteSpace(subject))
            {
                return await GetStudyGroups();
            }

            if (!Enum.TryParse<Subject>(subject, true, out var subjectEnum))
            {
                return new List<StudyGroup>();
            }

            return await Task.FromResult(_studyGroups.Values
                .Where(sg => sg.Subject == subjectEnum)
                .ToList());
        }

        public async Task JoinStudyGroup(int studyGroupId, int userId)
        {
            if (!_studyGroups.TryGetValue(studyGroupId, out var group))
                throw new ArgumentException("Study group not found.", nameof(studyGroupId));

            // Check if user already exists
            var existingUser = group.Users.FirstOrDefault(u => u.Id == userId);
            if (existingUser != null)
                return;

            // Add user using the method in StudyGroup.cs
            group.AddUser(new User { Id = userId, Name = "Unknown" });

            await Task.CompletedTask;
        }

        public async Task LeaveStudyGroup(int studyGroupId, int userId)
        {
            if (!_studyGroups.TryGetValue(studyGroupId, out var group))
                throw new ArgumentException("Study group not found.", nameof(studyGroupId));

            var userToRemove = group.Users.FirstOrDefault(u => u.Id == userId);
            if (userToRemove != null)
            {
                group.RemoveUser(userToRemove);
            }

            await Task.CompletedTask;
        }
    }
}
