using AutoMapper;
using BookStoreMVC.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentStorage.Models;
using StudentStorage.Models.Authentication;
using StudentStorage.Models.DTO;
using StudentStorage.Models.Enums;

namespace StudentStorage.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {

        IUnitOfWork _unitOfWork;
        UserManager<ApplicationUser> _userManager;
        IMapper _mapper;

        public CoursesController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
        }

        // GET api/v1/Courses
        [HttpGet]
        [ProducesResponseType(200)]
        [Authorize(Roles = UserRoles.Student)]
        public async Task<IActionResult> GetAll()
        {
            var courses = await _unitOfWork.Course.GetAllAsync(includeProperties: "Creator");
            IEnumerable<CourseResponseDTO> courseResponseDTOs = courses.Select(_mapper.Map<CourseResponseDTO>);
            return Ok(courseResponseDTOs);
        }

        // POST /api/v1/Courses/5/Requests
        [HttpPost("{courseId}/Requests")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [Authorize(Roles = UserRoles.Student)]
        public async Task<IActionResult> SendRequest(int courseId)
        {
            Course? course = await _unitOfWork.Course.GetByIdAsync(courseId);
            if (course == null)
            {
                return BadRequest();
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
            return Ok();
        }

        // GET api/v1/Courses/5/Requests/Pending
        [HttpGet("{id}/Requests/Pending")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [Authorize(Roles = UserRoles.Teacher)]
        public async Task<IActionResult> GetCourseRequests(int id)
        {
            Course? course = await _unitOfWork.Course.GetByIdAsync(id);
            if (course == null)
            {
                return BadRequest();
            }

            var requests = course.Requests;
            var pendingRequests = requests.Where(r => r.Status == CourseRequestStatus.Pending);
            var requestResponseDTOs = requests.Select(_mapper.Map<RequestResponseDTO>);
            return Ok(requestResponseDTOs);
        }

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
                return BadRequest();
            }
            CourseResponseDTO courseResponseDTO = _mapper.Map<CourseResponseDTO>(course);
            return Ok(courseResponseDTO);
        }

        // GET api/v1/Courses/5/Assignments
        [HttpGet("{id}/Assignments")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [Authorize(Roles = UserRoles.Student)]
        public async Task<IActionResult> GetCourseAssignments(int id)
        {
            Course? course = await _unitOfWork.Course.GetByIdAsync(id);
            if (course == null)
            {
                return BadRequest();
            }

            ICollection<Assignment> assignments = course.Assignments;
            IEnumerable<AssignmentResponseDTO> assignmentResponseDTOs = assignments.Select(_mapper.Map<AssignmentResponseDTO>);
            return Ok(assignmentResponseDTOs);
        }


        // POST api/v1/Courses
        [HttpPost]
        [ProducesResponseType(200)]
        [Authorize(Roles = UserRoles.Teacher)]
        public async Task<ActionResult> Post([FromBody] CourseRequestDTO courseDTO)
        {
            ApplicationUser? currentUser = await _userManager.GetUserAsync(HttpContext.User);
            Course course = new Course
            {
                CreatorId = currentUser.Id,
                Name = courseDTO.Name,
                Description = courseDTO.Description,
                CreatedAt = DateTime.Now
            };

            _unitOfWork.Course.AddAsync(course);
            await _unitOfWork.SaveAsync();
            CourseResponseDTO courseResponseDTO = _mapper.Map<CourseResponseDTO>(course);
            return Ok(courseResponseDTO);
        }

        // PUT api/v1/Courses/5
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [Authorize(Roles = UserRoles.Teacher)]
        public async Task<ActionResult> Put(int id, [FromBody] CourseRequestDTO courseDTO)
        {
            Course? course = await _unitOfWork.Course.GetByIdAsync(id);
            if (course == null)
            {
                return BadRequest();
            }
            course.Name = courseDTO.Name;
            course.Description = courseDTO.Description;
            await _unitOfWork.Course.UpdateAsync(course);
            await _unitOfWork.SaveAsync();
            CourseResponseDTO courseResponseDTO = _mapper.Map<CourseResponseDTO>(course);
            return Ok(courseResponseDTO);
        }

        // DELETE api/v1/Courses/5
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [Authorize(Roles = UserRoles.Teacher)]
        public async Task<IActionResult> Delete(int id)
        {
            Course? course = await _unitOfWork.Course.GetByIdAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            await _unitOfWork.Course.RemoveAsync(course);
            await _unitOfWork.SaveAsync();
            return Ok();
        }
    }
}
