using System.Collections.Generic;
using System.Threading.Tasks;
using TestApp;

namespace TestAppAPI
{
    public interface IStudyGroupRepository
    {
        Task CreateStudyGroup(StudyGroup studyGroup);
        Task<IEnumerable<StudyGroup>> GetStudyGroups();
        Task<IEnumerable<StudyGroup>> SearchStudyGroups(string subject);
        Task JoinStudyGroup(int studyGroupId, int userId);
        Task LeaveStudyGroup(int studyGroupId, int userId);
    }
}