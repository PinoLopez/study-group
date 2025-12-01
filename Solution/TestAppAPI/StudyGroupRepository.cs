using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestApp;

namespace TestAppAPI
{
    public class StudyGroupRepository : IStudyGroupRepository
    {
        private readonly AppDbContext _context;

        public StudyGroupRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateStudyGroup(StudyGroup studyGroup)
        {
            // Validate uniqueness by Subject
            if (await _context.StudyGroups.AnyAsync(s => s.Subject == studyGroup.Subject))
            {
                throw new InvalidOperationException($"A study group for subject '{studyGroup.Subject}' already exists.");
            }

            // Create new instance with current date (original is immutable)
            var newGroup = new StudyGroup(
                studyGroupId: 0, // Let EF generate the ID
                name: studyGroup.Name,
                subject: studyGroup.Subject,
                createDate: DateTime.UtcNow,
                users: new List<User>()
            );

            _context.StudyGroups.Add(newGroup);
            await _context.SaveChangesAsync();
        }

        public async Task<List<StudyGroup>> GetStudyGroups(string sort = "desc")
        {
            var query = _context.StudyGroups.Include(s => s.Users).AsQueryable();
            return sort.Equals("asc", StringComparison.OrdinalIgnoreCase)
                ? await query.OrderBy(s => s.CreateDate).ToListAsync()
                : await query.OrderByDescending(s => s.CreateDate).ToListAsync();
        }

        public async Task<List<StudyGroup>> SearchStudyGroups(string subject, string sort = "desc")
        {
            if (!Enum.TryParse<Subject>(subject, true, out var parsedSubject))
                throw new ArgumentException("Invalid subject.");

            var query = _context.StudyGroups
                .Where(s => s.Subject == parsedSubject)
                .Include(s => s.Users)
                .AsQueryable();

            return sort.Equals("asc", StringComparison.OrdinalIgnoreCase)
                ? await query.OrderBy(s => s.CreateDate).ToListAsync()
                : await query.OrderByDescending(s => s.CreateDate).ToListAsync();
        }

        public async Task JoinStudyGroup(int studyGroupId, int userId)
        {
            var group = await _context.StudyGroups
                .Include(s => s.Users)
                .FirstOrDefaultAsync(g => g.StudyGroupId == studyGroupId);
            
            var user = await _context.Users
                .Include(u => u.StudyGroups)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (group == null) throw new ArgumentException("Study group not found.");
            if (user == null) throw new ArgumentException("User not found.");
            if (group.Users.Any(u => u.Id == userId))
                throw new InvalidOperationException("User is already a member of this study group.");

            group.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task LeaveStudyGroup(int studyGroupId, int userId)
        {
            var group = await _context.StudyGroups
                .Include(s => s.Users)
                .FirstOrDefaultAsync(g => g.StudyGroupId == studyGroupId);
            
            var user = await _context.Users
                .Include(u => u.StudyGroups)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (group == null) throw new ArgumentException("Study group not found.");
            if (user == null) throw new ArgumentException("User not found.");
            if (!group.Users.Any(u => u.Id == userId))
                throw new ArgumentException("User is not a member of this study group.");

            group.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}