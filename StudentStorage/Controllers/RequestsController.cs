using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentStorage.DataAccess.Repository.IRepository;
using StudentStorage.Models;
using StudentStorage.Models.Authentication;
using StudentStorage.Models.DTO.Request;
using StudentStorage.Models.Enums;
using StudentStorage.Services;

namespace StudentStorage.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class RequestsController : ControllerBase
    {

        IUnitOfWork _unitOfWork;
        UserManager<ApplicationUser> _userManager;
        IMapper _mapper;
        CourseRequestService _courseRequestService;
        IAuthorizationService _authorizationService;
        FileManagerService _fileManagerService;

        public RequestsController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IMapper mapper, CourseRequestService courseRequestService, IAuthorizationService authorizationService, FileManagerService fileManagerService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
            _courseRequestService = courseRequestService;
            _authorizationService = authorizationService;
            _fileManagerService = fileManagerService;
        }

        /// <summary>
        /// Gets a specific request by ID. Only course authors can access this endpoint.
        /// </summary>
        /// <param name="id">
        /// Request ID
        /// </param>
        /// <returns>Returns a request</returns>
        // GET api/v1/Requests/5
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [Authorize(Roles = UserRoles.Teacher)]
        public async Task<IActionResult> Get(int id)
        {
            Request? request = await _unitOfWork.Request.GetByIdAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            var authorizationResult = await _authorizationService
            .AuthorizeAsync(User, request.Course, "CourseCreatorPolicy");

            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            RequestResponseDTO requestResponseDTO = _mapper.Map<RequestResponseDTO>(request);
            return Ok(requestResponseDTO);
        }

        /// <summary>
        /// Accept or deny a request. Only course authors can access this endpoint.
        /// </summary>
        /// <param name="id">
        /// Request ID
        /// </param>
        /// <param name="status">
        /// Status of the request (Approved or Denied)
        /// </param>
        // PUT api/v1/Requests/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] CourseRequestStatus status)
        {
            Request? request = await _unitOfWork.Request.GetByIdAsync(id);
            if (request == null)
            {
                return NotFound();
            }
            var result = await _courseRequestService.UpdateRequestStatus(request, status);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            if(status == CourseRequestStatus.Approved)
            {
                ApplicationUser user = await _userManager.FindByIdAsync(request.UserId.ToString());
                _fileManagerService.CreateStudentDirectory(request.Course, user);
            }
            

            return Ok();

        }
    }
}
