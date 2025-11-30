using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TestApp;

namespace TestAppAPI
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudyGroupController : ControllerBase
    {
        private readonly IStudyGroupRepository _studyGroupRepository;

        public StudyGroupController(IStudyGroupRepository studyGroupRepository)
        {
            _studyGroupRepository = studyGroupRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateStudyGroup([FromBody] StudyGroup studyGroup)
        {
            try
            {
                await _studyGroupRepository.CreateStudyGroup(studyGroup);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetStudyGroups([FromQuery] string sortOrder = "desc")
        {
            // Acceptance Criteria 3b: Allow sorting by creation date
            var studyGroups = await _studyGroupRepository.GetStudyGroups(sortOrder);
            return Ok(studyGroups);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchStudyGroups([FromQuery] string subject, [FromQuery] string sortOrder = "desc")
        {
            // Acceptance Criteria 3a: Filter by subject
            // Acceptance Criteria 3b: Sort results
            var studyGroups = await _studyGroupRepository.SearchStudyGroups(subject, sortOrder);
            return Ok(studyGroups);
        }

        [HttpPost("{studyGroupId}/join")]
        public async Task<IActionResult> JoinStudyGroup(int studyGroupId, [FromQuery] int userId, [FromQuery] string userName)
        {
            // Validate userName before calling repository
            if (string.IsNullOrWhiteSpace(userName))
            {
                return BadRequest("User name is required.");
            }

            try
            {
                // Acceptance Criteria 2: Users can join Study Groups
                await _studyGroupRepository.JoinStudyGroup(studyGroupId, userId, userName);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("{studyGroupId}/leave")]
        public async Task<IActionResult> LeaveStudyGroup(int studyGroupId, [FromQuery] int userId)
        {
            try
            {
                // Acceptance Criteria 4: Users can leave Study Groups
                await _studyGroupRepository.LeaveStudyGroup(studyGroupId, userId);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}