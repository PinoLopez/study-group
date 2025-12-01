using Microsoft.AspNetCore.Mvc;
using System;
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
        public async Task<IActionResult> CreateStudyGroup(StudyGroup studyGroup)
        {
            if (studyGroup == null)
                return BadRequest("Study group is required.");

            try
            {
                await _studyGroupRepository.CreateStudyGroup(studyGroup);
                return Ok();
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetStudyGroups([FromQuery] string sort = "desc")
        {
            var groups = await _studyGroupRepository.GetStudyGroups(sort);
            return Ok(groups);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchStudyGroups([FromQuery] string subject, [FromQuery] string sort = "desc")
        {
            if (string.IsNullOrWhiteSpace(subject))
                return BadRequest("Subject is required.");

            try
            {
                var groups = await _studyGroupRepository.SearchStudyGroups(subject, sort);
                return Ok(groups);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("join")]
        public async Task<IActionResult> JoinStudyGroup([FromQuery] int studyGroupId, [FromQuery] int userId)
        {
            try
            {
                await _studyGroupRepository.JoinStudyGroup(studyGroupId, userId);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("leave")]
        public async Task<IActionResult> LeaveStudyGroup([FromQuery] int studyGroupId, [FromQuery] int userId)
        {
            try
            {
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