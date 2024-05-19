using AutoMapper;
using StudentStorage.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    public class UsersController : ControllerBase
    {
        IUnitOfWork _unitOfWork;
        UserManager<ApplicationUser> _userManager;
        IMapper _mapper;

        public UsersController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
        }

        // GET api/Users/me/
        [HttpGet("me")]
        [Authorize(Roles = UserRoles.Student)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetCurrentUser()
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            UserDTO userDTO = _mapper.Map<UserDTO>(user);
            return Ok(userDTO);
        }

        // GET api/Users/5/Courses
        [HttpGet("{userId}/Courses")]
        [Authorize(Roles = UserRoles.Student)]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetUserCourses(string userId)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return BadRequest();
            }
            IEnumerable<Course> courses = await _unitOfWork.User.GetUserCreatedCoursesAsync(userId);
            IEnumerable<CourseResponseDTO> courseResponseDTOs = courses.Select(_mapper.Map<CourseResponseDTO>);
            return Ok(courseResponseDTOs);
        }

        // GET api/Users/5/Requests/Pending
        [HttpGet("{userId}/Requests/Pending")]
        [Authorize(Roles = UserRoles.Student)]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetUserRequests(string userId)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return BadRequest();
            }
            IEnumerable<Request> requests = await _unitOfWork.User.GetRequestsAsync(userId);
            var pendingRequests = requests.Where(r => r.Status == CourseRequestStatus.Pending);
            IEnumerable<RequestResponseDTO> requestResponseDTOs = requests.Select(_mapper.Map<RequestResponseDTO>);
            return Ok(requestResponseDTOs);
        }

        // GET api/Users/5/Solutions

    }
}
