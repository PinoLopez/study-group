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
            // Acceptance Criteria 1: Only one Study Group for a single Subject
            if (_studyGroups.Values.Any(sg => sg.Subject == studyGroup.Subject))
            {
                throw new InvalidOperationException($"A study group for subject '{studyGroup.Subject}' already exists.");
            }

            // Simulating generating an ID
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

        public async Task<IEnumerable<StudyGroup>> GetStudyGroups(string sortOrder = "desc")
        {
            // Acceptance Criteria 3b: Sort by creation date (most recent or oldest)
            var groups = sortOrder?.ToLower() == "asc" 
                ? _studyGroups.Values.OrderBy(x => x.CreateDate).ToList()
                : _studyGroups.Values.OrderByDescending(x => x.CreateDate).ToList();
                
            return await Task.FromResult(groups);
        }

        public async Task<IEnumerable<StudyGroup>> SearchStudyGroups(string subject, string sortOrder = "desc")
        {
            // Acceptance Criteria 3a: Filter by subject
            if (string.IsNullOrWhiteSpace(subject))
            {
                return await GetStudyGroups(sortOrder);
            }

            if (!Enum.TryParse<Subject>(subject, true, out var subjectEnum))
            {
                return new List<StudyGroup>();
            }

            var groups = _studyGroups.Values
                .Where(sg => sg.Subject == subjectEnum);

            // Acceptance Criteria 3b: Sort by creation date
            var sortedGroups = sortOrder?.ToLower() == "asc"
                ? groups.OrderBy(x => x.CreateDate).ToList()
                : groups.OrderByDescending(x => x.CreateDate).ToList();

            return await Task.FromResult(sortedGroups);
        }

        public async Task JoinStudyGroup(int studyGroupId, int userId, string userName)
        {
            // Acceptance Criteria 2: Users can join Study Groups
            if (!_studyGroups.TryGetValue(studyGroupId, out var group))
                throw new ArgumentException("Study group not found.", nameof(studyGroupId));

            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("User name is required.", nameof(userName));

            // Check if user already exists in the group
            var existingUser = group.Users.FirstOrDefault(u => u.Id == userId);
            if (existingUser != null)
                throw new InvalidOperationException("User is already a member of this study group.");

            // Add user with proper information
            group.AddUser(new User { Id = userId, Name = userName });
            await Task.CompletedTask;
        }

        public async Task LeaveStudyGroup(int studyGroupId, int userId)
        {
            // Acceptance Criteria 4: Users can leave Study Groups
            if (!_studyGroups.TryGetValue(studyGroupId, out var group))
                throw new ArgumentException("Study group not found.", nameof(studyGroupId));

            var userToRemove = group.Users.FirstOrDefault(u => u.Id == userId);
            if (userToRemove == null)
                throw new ArgumentException("User is not a member of this study group.", nameof(userId));

            group.RemoveUser(userToRemove);
            await Task.CompletedTask;
        }
    }
}