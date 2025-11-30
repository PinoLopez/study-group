// File: ~/Documents/webapp/Solution/TestAppAPI/StudyGroupController.cs
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
        public async Task<IActionResult> GetStudyGroups()
        {
            var studyGroups = await _studyGroupRepository.GetStudyGroups();
            return Ok(studyGroups);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchStudyGroups(string subject)
        {
            try
            {
                var studyGroups = await _studyGroupRepository.SearchStudyGroups(subject);
                return Ok(studyGroups);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
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
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex) when (ex.Message.Contains("not found") || ex.Message.Contains("not a member"))
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
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
            catch (ArgumentException ex) when (ex.Message.Contains("not found") || ex.Message.Contains("not a member"))
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}