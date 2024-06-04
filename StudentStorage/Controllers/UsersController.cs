using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentStorage.DataAccess.Repository.IRepository;
using StudentStorage.Models;
using StudentStorage.Models.Authentication;
using StudentStorage.Models.DTO.Course;
using StudentStorage.Models.DTO.Request;
using StudentStorage.Models.DTO.User;
using StudentStorage.Models.Enums;

namespace StudentStorage.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        IUnitOfWork _unitOfWork;
        UserManager<ApplicationUser> _userManager;
        IMapper _mapper;
        IAuthorizationService _authorizationService;

        public UsersController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IMapper mapper, IAuthorizationService authorizationService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
            _authorizationService = authorizationService;
        }

        /// <summary>
        /// Gets user by ID. Students can only access their own data.
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>User DTO</returns>
        // GET api/Users/5/
        [HttpGet("{id}")]
        [Authorize(Roles = UserRoles.Student)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Get(int id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            var authorizationResult = await _authorizationService
            .AuthorizeAsync(User, "CourseMembershipPolicy");
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            UserDTO userDTO = _mapper.Map<UserDTO>(user);
            return Ok(userDTO);
        }

        /// <summary>
        /// Gets all user courses. Students can only access their own courses.
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>Array of DTOs for every course that user is enrolled to.</returns>
        // GET api/Users/5/Courses
        [HttpGet("{id}/Courses")]
        [Authorize(Roles = UserRoles.Student)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetCourses(int id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            var authorizationResult = await _authorizationService
            .AuthorizeAsync(User, "CourseMembershipPolicy");
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            IEnumerable<Course> courses = await _unitOfWork.User.GetUserCreatedCoursesAsync(id);
            IEnumerable<CourseResponseDTO> courseResponseDTOs = courses.Select(_mapper.Map<CourseResponseDTO>);
            return Ok(courseResponseDTOs);
        }

        /// <summary>
        /// Gets all pending requests sent by user.
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>All users pending requests.</returns>
        // GET api/Users/5/Requests/Pending
        [HttpGet("{id}/Requests/Pending")]
        [Authorize(Roles = UserRoles.Student)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetPendingRequests(int id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return BadRequest();
            }

            var authorizationResult = await _authorizationService
            .AuthorizeAsync(User, "CourseMembershipPolicy");
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            IEnumerable<Request> requests = await _unitOfWork.User.GetRequestsAsync(id);
            var pendingRequests = requests.Where(r => r.Status == CourseRequestStatus.Pending);
            IEnumerable<RequestResponseDTO> requestResponseDTOs = requests.Select(_mapper.Map<RequestResponseDTO>);
            return Ok(requestResponseDTOs);
        }
    }
}
