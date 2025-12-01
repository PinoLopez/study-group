using Microsoft.AspNetCore.Mvc;
using TestApp;
using TestAppAPI;

namespace TestAppAPI
{
    [ApiController]
    [Route("studygroup")]
    public class StudyGroupController : ControllerBase
    {
        private readonly IStudyGroupRepository _studyGroupRepository;

        public StudyGroupController(IStudyGroupRepository studyGroupRepository)
        {
            _studyGroupRepository = studyGroupRepository;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateStudyGroup(StudyGroup? studyGroup)
        {
            // AC: Null input → 400 Bad Request (as BadRequestObjectResult)
            if (studyGroup == null)
            {
                return BadRequest("Study group cannot be null.");
            }

            try
            {
                await _studyGroupRepository.CreateStudyGroup(studyGroup);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                // e.g., "A study group for subject 'X' already exists."
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                // e.g., name length validation
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetStudyGroups(string? sort = "desc")
        {
            try
            {
                var studyGroups = await _studyGroupRepository.GetStudyGroups(sort ?? "desc");
                return Ok(studyGroups);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchStudyGroups(string subject, string? sort = "desc")
        {
            if (string.IsNullOrWhiteSpace(subject))
            {
                return BadRequest("Subject is required.");
            }

            try
            {
                var studyGroups = await _studyGroupRepository.SearchStudyGroups(subject, sort ?? "desc");
                return Ok(studyGroups);
            }
            catch (ArgumentException)
            {
                // Invalid subject (e.g., "Biology") → 400 Bad Request
                return BadRequest("Invalid subject.");
            }
        }

        [HttpPost("join")]
        public async Task<IActionResult> JoinStudyGroup(int studyGroupId, int userId)
        {
            try
            {
                await _studyGroupRepository.JoinStudyGroup(studyGroupId, userId);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                // e.g., "User is already a member"
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                // "Study group not found" or "User not found" → 404
                if (ex.Message.Contains("Study group not found") || ex.Message.Contains("User not found"))
                {
                    return NotFound(ex.Message);
                }
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("leave")]
        public async Task<IActionResult> LeaveStudyGroup(int studyGroupId, int userId)
        {
            try
            {
                await _studyGroupRepository.LeaveStudyGroup(studyGroupId, userId);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                // "Study group not found" or "User is not a member" → 404
                if (ex.Message.Contains("Study group not found") || ex.Message.Contains("User is not a member"))
                {
                    return NotFound(ex.Message);
                }
                return BadRequest(ex.Message);
            }
        }

        // === TEMPORARY DEBUG ENDPOINT ===
        [HttpGet("debug/query-m-users")]
        public async Task<IActionResult> GetStudyGroupsWithMUsers()
        {
            var studyGroups = await _studyGroupRepository.GetStudyGroupsWithUsersNamedStartingWithM();
            return Ok(studyGroups);
        }
    }
}