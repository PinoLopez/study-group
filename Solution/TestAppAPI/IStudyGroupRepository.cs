using System.Collections.Generic;
using System.Threading.Tasks;
using TestApp;

namespace TestAppAPI
{
    public interface IStudyGroupRepository
    {
        Task CreateStudyGroup(StudyGroup studyGroup);
        Task<List<StudyGroup>> GetStudyGroups(string sort = "desc");
        Task<List<StudyGroup>> SearchStudyGroups(string subject, string sort = "desc");
        Task JoinStudyGroup(int studyGroupId, int userId);
        Task LeaveStudyGroup(int studyGroupId, int userId);
        Task<List<StudyGroup>> GetStudyGroupsWithUsersNamedStartingWithM();
    }
}