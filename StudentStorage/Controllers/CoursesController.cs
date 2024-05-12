using AutoMapper;
using BookStoreMVC.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentStorage.Models;
using StudentStorage.Models.Authentication;
using StudentStorage.Models.DTO;
using StudentStorage.Models.Enums;
using System.Security.Claims;

namespace StudentStorage.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {

        IUnitOfWork _unitOfWork;
        UserManager<ApplicationUser> _userManager;
        IMapper _mapper;
        IAuthorizationService _authorizationService;

        public CoursesController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IMapper mapper, IAuthorizationService authorizationService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
            _authorizationService = authorizationService;
        }

        /// <summary>
        /// Gets all courses.
        /// </summary>
        /// <returns>Returns an array of all courses</returns>
        /// <response code="200">Returns the created courses.</response>
        /// <response code="401">Access denied (Unauthorized)</response>
        /// <returns>
        /// A 200 OK response containing an array of all accessible courses, or a 401 Unauthorized response if the user is not authenticated or does not have the Student role.
        /// Each course in the array is represented as a CourseResponseDTO, which includes the ID, name, and description of the course, as well as the ID of the creator.
        /// </returns>
        // GET api/v1/Courses
        [HttpGet]
        [ProducesResponseType(typeof(List<CourseResponseDTO>), 200)]
        [ProducesResponseType(401)]
        [Authorize(Roles = UserRoles.Student)]
        public async Task<IActionResult> GetAll()
        {
            var courses = await _unitOfWork.Course.GetAllAsync(includeProperties: "Creator");
            IEnumerable<CourseResponseDTO> courseResponseDTOs = courses.Select(_mapper.Map<CourseResponseDTO>);
            return Ok(courseResponseDTOs);
        }

        /// <summary>
        /// Gets a specific course by ID, including all the assignments.
        /// </summary>
        /// <param name="id">
        /// Course ID
        /// </param>
        /// <returns>Returns a course</returns>
        // GET api/v1/Courses/5
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [Authorize(Roles = UserRoles.Student)]
        public async Task<IActionResult> Get(int id)
        {
            Course? course = await _unitOfWork.Course.GetByIdAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            var authorizationResult = await _authorizationService
            .AuthorizeAsync(User, course, "CourseMembershipPolicy");

            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }
            CourseDetailResponseDTO courseResponseDTO = _mapper.Map<CourseDetailResponseDTO>(course);
            return Ok(courseResponseDTO);
        }

        /// <summary>
        /// Sends a request for the course creator to join the course.
        /// </summary>
        /// <param name="id">
        /// Course ID
        /// </param>
        // POST /api/v1/Courses/5/Requests
        [HttpPost("{id}/Requests")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [Authorize(Roles = UserRoles.Student)]
        public async Task<IActionResult> SendRequest(int id)
        {
            Course? course = await _unitOfWork.Course.GetByIdAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            ApplicationUser? currentUser = await _userManager.GetUserAsync(HttpContext.User);
            Request request = new Request
            {
                CourseId = course.Id,
                UserId = currentUser.Id,
                Status = CourseRequestStatus.Pending,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            await _unitOfWork.Request.AddAsync(request);
            await _unitOfWork.SaveAsync();
            JoinRequestDTO joinRequestDTO = _mapper.Map<JoinRequestDTO>(request);
            return Ok(joinRequestDTO);
        }

        /// <summary>
        /// Gets all the assignments for the course.
        /// </summary>
        /// <param name="id">
        /// Course ID
        /// </param>
        /// <returns>Returns an array of all assignment from the course</returns>
        // GET api/v1/Courses/5/Assignments
        [HttpGet("{id}/Assignments")]
        [ProducesResponseType(typeof(IEnumerable<AssignmentResponseDTO>), 200)]
        [ProducesResponseType(400)]
        [Authorize(Roles = UserRoles.Student)]
        public async Task<IActionResult> GetCourseAssignments(int id)
        {
            Course? course = await _unitOfWork.Course.GetByIdAsync(id);
            if (course == null)
            {
                return BadRequest();
            }

            var authorizationResult = await _authorizationService
            .AuthorizeAsync(User, course, "CourseMembershipPolicy");

            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            ICollection<Assignment> assignments = course.Assignments;
            IEnumerable<AssignmentResponseDTO> assignmentResponseDTOs = assignments.Select(_mapper.Map<AssignmentResponseDTO>);
            return Ok(assignmentResponseDTOs);
        }

        /// <summary>
        /// Gets pending request for the course.
        /// </summary>
        /// <param name="id">
        /// Course ID
        /// </param>
        /// <returns>Returns an array of all pending requests</returns>
        // GET api/v1/Courses/5/Requests/Pending
        [HttpGet("{id}/Requests/Pending")]
        [ProducesResponseType(typeof(List<RequestResponseDTO>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [Authorize(Roles = UserRoles.Teacher)]
        public async Task<IActionResult> GetCoursePendingRequests(int id)
        {
            Course? course = await _unitOfWork.Course.GetByIdAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (course.CreatorId != userId)
            {
                return Forbid();
            }
            var requests = course.Requests;
            var pendingRequests = requests.Where(r => r.Status == CourseRequestStatus.Pending);
            var requestResponseDTOs = requests.Select(_mapper.Map<RequestResponseDTO>);
            return Ok(requestResponseDTOs);
        }

        /// <summary>
        /// Creates a course.
        /// </summary>
        /// <param name="courseDTO">
        /// A course request DTO.
        /// </param>
        // POST api/v1/Courses
        [HttpPost]
        [ProducesResponseType(typeof(CourseResponseDTO), 200)]
        [ProducesResponseType(401)]
        [Authorize(Roles = UserRoles.Teacher)]
        public async Task<ActionResult> Post([FromBody] CourseRequestDTO courseDTO)
        {
            string userId = _userManager.GetUserId(User);
            Course course = new Course
            {
                CreatorId = userId,
                Name = courseDTO.Name,
                Description = courseDTO.Description,
                CreatedAt = DateTime.Now
            };
            await _unitOfWork.Course.AddAsync(course);
            await _unitOfWork.SaveAsync();
            CourseResponseDTO courseResponseDTO = _mapper.Map<CourseResponseDTO>(course);
            return Ok(courseResponseDTO);
        }

        /// <summary>
        /// Modifies a course.
        /// </summary>
        /// <param name="id">
        /// Course ID.
        /// </param>
        /// <param name="courseDTO">
        /// Course request DTO.
        /// </param>
        // PUT api/v1/Courses/5
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(CourseResponseDTO), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [Authorize(Roles = UserRoles.Teacher)]
        public async Task<ActionResult> Put(int id, [FromBody] CourseRequestDTO courseDTO)
        {
            Course? course = await _unitOfWork.Course.GetByIdAsync(id);
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (course == null)
            {
                return NotFound();
            }
            if (course.CreatorId != userId)
            {
                return Forbid();
            }
            course.Name = courseDTO.Name;
            course.Description = courseDTO.Description;

            await _unitOfWork.Course.UpdateAsync(course);
            await _unitOfWork.SaveAsync();
            CourseResponseDTO courseResponseDTO = _mapper.Map<CourseResponseDTO>(course);
            return Ok(courseResponseDTO);
        }

        /// <summary>
        /// Deletes a course.
        /// </summary>
        /// <param name="id">
        /// Course ID.
        /// </param>
        // DELETE api/v1/Courses/5
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [Authorize(Roles = UserRoles.Teacher)]
        public async Task<IActionResult> Delete(int id)
        {
            Course? course = await _unitOfWork.Course.GetByIdAsync(id);
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (course == null)
            {
                return NotFound();
            }
            if (course.CreatorId != userId)
            {
                return Forbid();
            }
            await _unitOfWork.Course.RemoveAsync(course);
            await _unitOfWork.SaveAsync();
            return Ok();
        }
    }
}
