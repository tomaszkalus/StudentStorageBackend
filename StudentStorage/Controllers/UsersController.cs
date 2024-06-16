using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using StudentStorage.DataAccess.Repository.IRepository;
using StudentStorage.Models;
using StudentStorage.Models.DTO.Course;
using StudentStorage.Models.DTO.Request;
using StudentStorage.Models.DTO.Solution;
using StudentStorage.Models.DTO.User;
using StudentStorage.Models.Enums;
using StudentStorage.Services;

namespace StudentStorage.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    IUnitOfWork _unitOfWork;
    UserManager<ApplicationUser> _userManager;
    IMapper _mapper;
    IAuthorizationService _authorizationService;
    AccountService _accountService;
    AssignmentSolutionService _assignmentSolutionService;

    public UsersController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IMapper mapper, IAuthorizationService authorizationService, AccountService accountService, AssignmentSolutionService assignmentSolutionService)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _mapper = mapper;
        _authorizationService = authorizationService;
        _accountService = accountService;
        _assignmentSolutionService = assignmentSolutionService;
    }

    
    /// <summary>
    /// Deletes user by ID. Users can only delete their own account. Admins can delete any account.
    /// </summary>
    /// <param name="id">User ID</param>
    // DELETE api/Users/5
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        ApplicationUser user = await _userManager.FindByIdAsync(id.ToString());

        if (user == null)
        {
            return NotFound();
        }

        var authorizationResult = await _authorizationService
        .AuthorizeAsync(User, "SameUserPolicy");
        if (!authorizationResult.Succeeded)
        {
            return Forbid();
        }

        await _unitOfWork.User.RemoveAsync(user);
        await _unitOfWork.CommitAsync();
        return NoContent();

    }

    /// <summary>
    /// Gets all users. Only admins can access this endpoint.
    /// </summary>
    /// <returns code="200">List of DTOs of all the application users</returns>
    // GET api/Users
    [HttpGet]
    [Authorize(Roles = UserRoles.Admin)]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetAll()
    {
        IEnumerable<ApplicationUser> users = await _unitOfWork.User.GetAllAsync();
        List<UserDetailsDto> userDTOs = new List<UserDetailsDto>();

        foreach (var user in users)
        {
            var userAdminDto = new UserDetailsDto
            {
                Id = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = await _accountService.GetHighestRankingRoleAsync(user),
                CreatedAt = user.CreatedAt
            };
            userDTOs.Add(userAdminDto);
        }

        return Ok(userDTOs);
    }

    /// <summary>
    /// Gets user by ID. Students can only access their own data.
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>User DTO</returns>
    /// <response code="200">If user requests his own userID or admin requests any existing user.</response>
    /// <response code="404">If user has not been found.</response>
    /// <response code="401">If user without admin privilages tries to access other user.</response>
    // GET api/Users/5/
    [HttpGet("{id}")]
    [Authorize(Roles = UserRoles.Student)]
    [ProducesResponseType(typeof(UserDetailsDto), 200)]
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
        .AuthorizeAsync(User, "SameUserPolicy");
        if (!authorizationResult.Succeeded)
        {
            return Forbid();
        }

        UserDetailsDto dto = new UserDetailsDto
        {
            Id = user.Id,
            UserName = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = await _accountService.GetHighestRankingRoleAsync(user),
            CreatedAt = user.CreatedAt
        };

        return Ok(dto);
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
            return Forbid("User can only access their own courses.");
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
    /// <response code="201">Returns the newly created item</response>
    /// <response code="400">If the item is null</response>
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

    /// <summary>
    /// Retrieves all user's solutions for a given assignment.
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="assignmentId">Assignment ID</param>
    /// <returns>Collection of solutions DTOs for the assignment provided by the user.</returns>
    // GET api/v1/Users/5/Assignments/5/Solutions/Details
    [HttpGet("{id}/Assignments/{assignmentId}/Solutions/Details")]
    [Authorize(Roles = UserRoles.Student)]
    public async Task<IActionResult> GetSolutions(int id, int assignmentId)
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
        var solutions = await _unitOfWork.Solution.GetAllUserAssignmentSolutions(id, currentUser.Id);

        IEnumerable<SolutionResponseDTO> solutionResponseDTOs = solutions.Select(e =>
        {
            return new SolutionResponseDTO
            {
                Id = e.Id,
                SizeMb = e.SizeMb,
                FileName = Path.GetFileName(e.FilePath)
            };
        });
        return Ok(solutionResponseDTOs);
    }

    /// <summary>
    /// Gets user solutions for a given assignment, compressed to a zip file.
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="assignmentId">Assignment ID</param>
    /// <response code="200">If the request was successful</response>
    /// <response code="404">If the user, assignment or solutions have not been found.</response>
    /// <response code="403">If the user is not a member of the course</response>
    /// <returns>ZIP file containing user solutions for the assignment</returns>
    // GET api/v1/Users/5/Assignments/4/Solutions/zip
    [HttpGet("{id}/Assignments/{assignmentId}/Solutions/zip")]
    [Authorize(Roles = UserRoles.Teacher)]
    [ProducesResponseType(typeof(FileStreamResult), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> GetUserAssignmentSolutions(int id, int assignmentId)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());

        if (user == null)
        {
            return NotFound("The user was not found");
        }

        Assignment? assignment = await _unitOfWork.Assignment.GetByIdAsync(assignmentId);
        if (assignment == null)
        {
            return NotFound("The assignment was not found");
        }

        var authorizationResult = await _authorizationService
        .AuthorizeAsync(User, assignment.Course, "CourseMembershipPolicy");
        if (!authorizationResult.Succeeded) 
        {
            return Forbid("The user is not a member of the course");
        }

        IEnumerable<Solution> solutions = await _unitOfWork.Solution.GetAllUserAssignmentSolutions(assignmentId, id);

        if(solutions.Count() == 0)
        {
            return NotFound("The user has not submitted any solutions for this assignment");
        }

        IFormFile archive = _assignmentSolutionService.GetUserAssignmentSolutionsArchive(solutions);

        if (archive == null)
        {
            return NotFound();
        }

        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(archive.FileName, out var mimeType))
        {
            mimeType = "application/octet-stream";
        }

        return File(archive.OpenReadStream(), mimeType, archive.FileName);
    }
}

