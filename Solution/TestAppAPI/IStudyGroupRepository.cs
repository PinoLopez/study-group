using System.Collections.Generic;
using System.Threading.Tasks;
using TestApp;

namespace TestAppAPI
{
    public interface IStudyGroupRepository
    {
        Task CreateStudyGroup(StudyGroup studyGroup);
        Task<IEnumerable<StudyGroup>> GetStudyGroups(string sortOrder = "desc");
        Task<IEnumerable<StudyGroup>> SearchStudyGroups(string subject, string sortOrder = "desc");
        Task JoinStudyGroup(int studyGroupId, int userId, string userName);
        Task LeaveStudyGroup(int studyGroupId, int userId);
    }
}