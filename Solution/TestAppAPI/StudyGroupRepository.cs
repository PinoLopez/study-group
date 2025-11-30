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
            if (await _context.StudyGroups.AnyAsync(s => s.Subject == studyGroup.Subject))
            {
                throw new InvalidOperationException($"A study group for subject '{studyGroup.Subject}' already exists.");
            }

            studyGroup.CreateDate = DateTime.UtcNow;
            _context.StudyGroups.Add(studyGroup);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<StudyGroup>> GetStudyGroups()
        {
            return await _context.StudyGroups
                .Include(s => s.Users)
                .OrderByDescending(s => s.CreateDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<StudyGroup>> SearchStudyGroups(string subject)
        {
            if (!Enum.TryParse<Subject>(subject, true, out var parsedSubject))
            {
                throw new ArgumentException("Invalid subject.");
            }

            return await _context.StudyGroups
                .Where(s => s.Subject == parsedSubject)
                .Include(s => s.Users)
                .OrderByDescending(s => s.CreateDate)
                .ToListAsync();
        }

        public async Task JoinStudyGroup(int studyGroupId, int userId)
        {
            var group = await _context.StudyGroups.Include(s => s.Users).FirstOrDefaultAsync(s => s.StudyGroupId == studyGroupId);
            var user = await _context.Users.FindAsync(userId);

            if (group == null || user == null) throw new KeyNotFoundException();

            group.AddUser(user);
            await _context.SaveChangesAsync();
        }

        public async Task LeaveStudyGroup(int studyGroupId, int userId)
        {
            var group = await _context.StudyGroups.Include(s => s.Users).FirstOrDefaultAsync(s => s.StudyGroupId == studyGroupId);
            var user = await _context.Users.FindAsync(userId);

            if (group == null || user == null) throw new KeyNotFoundException();

            group.RemoveUser(user);
            await _context.SaveChangesAsync();
        }
    }
}