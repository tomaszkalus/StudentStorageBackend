using AutoMapper;
using StudentStorage.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentStorage.Models;
using StudentStorage.Models.DTO;
using StudentStorage.Models.DTO.Solution;
using StudentStorage.Services;
using StudentStorage.Models.Enums;
using StudentStorage.Models.Responses;
using StudentStorage.Models.DTO.User;

namespace StudentStorage.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AssignmentsController : ControllerBase
    {
        IUnitOfWork _unitOfWork;
        UserManager<ApplicationUser> _userManager;
        IMapper _mapper;
        AssignmentSolutionService _assignmentSolutionService;
        IAuthorizationService _authorizationService;

        public AssignmentsController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IMapper mapper, AssignmentSolutionService assignmentSolutionService, IAuthorizationService authorizationService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
            _assignmentSolutionService = assignmentSolutionService;
            _authorizationService = authorizationService;
        }

        #region Assignment

        /// <summary>
        /// Gets all assignments.
        /// </summary>
        /// <returns>Returns an array of all assignments</returns>
        /// <response code="200">Returns the created assignment.</response>
        /// <response code="401">Access denied (Unauthorized)</response>
        // GET api/v1/Assignments
        [HttpGet]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> GetAll()
        {
            var Assignments = await _unitOfWork.Assignment.GetAllAsync(null, includeProperties: "Creator");
            IEnumerable<AssignmentResponseDTO> AssignmentResponseDTOs = Assignments.Select(_mapper.Map<AssignmentResponseDTO>);
            return Ok(AssignmentResponseDTOs);
        }

        /// <summary>
        /// Gets a specific assignment by ID.
        /// </summary>
        /// <param name="id">
        /// Assignment ID.
        /// </param>
        /// <returns>Returns an assignment</returns>
        // GET api/Assignments/5
        [HttpGet("{id}")]
        [Authorize(Roles = UserRoles.Student)]
        public async Task<IActionResult> Get(int id)
        {

            Assignment? Assignment = await _unitOfWork.Assignment.GetByIdAsync(id);
            if (Assignment == null)
            {
                return BadRequest();
            }
            AssignmentResponseDTO AssignmentResponseDTO = _mapper.Map<AssignmentResponseDTO>(Assignment);
            return Ok(AssignmentResponseDTO);
        }

        /// <summary>
        /// Updates an assignment.
        /// </summary>
        /// <param name="id">
        /// Assignment ID.
        /// </param>
        /// <param name="AssignmentDTO">
        /// Assignment object.
        /// </param>
        /// <returns>Returns an updated assignment</returns>
        // PUT api/Assignments/5
        [HttpPut("{id}")]
        [Authorize(Roles = UserRoles.Teacher)]
        public async Task<ActionResult> Put(int id, [FromBody] AssignmentRequestDTO AssignmentDTO)
        {
            Assignment? Assignment = await _unitOfWork.Assignment.GetByIdAsync(id);
            if (Assignment == null)
            {
                return BadRequest();
            }
            Assignment.Title = AssignmentDTO.Title;
            Assignment.Description = AssignmentDTO.Description;
            Assignment.DueDate = AssignmentDTO.DueDate;
            Assignment.AllowLateSubmissions = AssignmentDTO.AllowLateSubmissions;
            Assignment.Hidden = AssignmentDTO.Hidden;

            await _unitOfWork.Assignment.UpdateAsync(Assignment);
            await _unitOfWork.CommitAsync();
            AssignmentResponseDTO AssignmentResponseDTO = _mapper.Map<AssignmentResponseDTO>(Assignment);
            return Ok(AssignmentResponseDTO);
        }

        /// <summary>
        /// Deletes an assignment.
        /// </summary>
        /// <param name="id">
        /// Assignment ID.
        /// </param>
        /// <response code="200">The delete operation was successful.</response>
        /// <response code="404">No assignment with that Id was found</response>
        /// <response code="403">Access denied (Unauthorized)</response>
        // DELETE api/Assignments/5
        [HttpDelete("{id}")]
        [Authorize(Roles = UserRoles.Teacher)]
        public async Task<IActionResult> Delete(int id)
        {
            Assignment? Assignment = await _unitOfWork.Assignment.GetByIdAsync(id);
            if (Assignment == null)
            {
                return NotFound();
            }
            await _unitOfWork.Assignment.RemoveAsync(Assignment);
            await _unitOfWork.CommitAsync();
            return Ok();
        }

        

        #endregion Assignment
        #region Solution

        /// <summary>
        /// Adds a solution to an assignment. A solution contains one or more files.
        /// </summary>
        /// <param name="id">Assignment ID</param>
        /// <param name="SolutionDTO">DTO of a solution provided by user.</param>
        /// <returns code="200">If the operation was successful.</returns>
        /// <returns code="400">If the there was an error.</returns>
        /// <returns code="403">If the assignment belongs to the course that the user is not a member of.</returns>
        // POST api/Assignments/5/Solutions
        [HttpPost("{id}/Solutions")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [Authorize(Roles = UserRoles.Student)]
        public async Task<IActionResult> AddSolution(int id, [FromForm] SolutionRequestDTO SolutionDTO)
        {
            Assignment? assignment = await _unitOfWork.Assignment.GetByIdAsync(id);
            if (assignment == null)
            {
                return BadRequest();
            }

            var authorizationResult = await _authorizationService
            .AuthorizeAsync(User, assignment.Course, "CourseMembershipPolicy");
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var result = await _assignmentSolutionService.SubmitAssignmentSolutionFilesAsync(SolutionDTO, assignment, currentUser);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok();
        }

        /// <summary>
        /// Retrieves all course members with the summary about their solutions for a given assignment.
        /// </summary>
        /// <param name="id">Assignment ID</param>
        /// <returns></returns>
        // GET api/Assignments/5/Solutions
        [HttpGet("{id}/Solutions")]
        [Authorize(Roles = UserRoles.Teacher)]
        public async Task<IActionResult> GetAssignmentSolutionSummary(int id)
        {
            Assignment? assignment = await _unitOfWork.Assignment.GetByIdAsync(id);
            if (assignment == null)
            {
                return NotFound();
            }

            var authorizationResult = await _authorizationService
            .AuthorizeAsync(User, assignment.Course, "CourseMembershipPolicy");
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            List<UserSolutionsSummaryDTO> userSolutionDTOs = new List<UserSolutionsSummaryDTO>();
            IEnumerable<ApplicationUser> students = await _unitOfWork.User.GetCourseMembers(assignment.CourseId);

            foreach(var student in students)
            {
                var solutions = await _unitOfWork.Solution.GetAllUserAssignmentSolutions(id, student.Id);
                userSolutionDTOs.Add(new UserSolutionsSummaryDTO
                {
                    Id = student.Id,
                    UserName = student.UserName,
                    FirstName = student.FirstName,
                    LastName = student.LastName,
                    SolutionStatus = _assignmentSolutionService.GetSolutionStatus(solutions).ToString()
                });
            }
            return Ok(userSolutionDTOs);
        }

        #endregion Solution



    }
}
