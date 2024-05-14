using AutoMapper;
using StudentStorage.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentStorage.Models;
using StudentStorage.Models.Authentication;
using StudentStorage.Models.DTO;
using StudentStorage.Models.Enums;
using StudentStorage.Services;
using System.Security.Claims;
using StudentStorage.Models.Exceptions;

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

        public RequestsController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IMapper mapper, CourseRequestService courseRequestService, IAuthorizationService authorizationService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
            _courseRequestService = courseRequestService;
            _authorizationService = authorizationService;
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
                return BadRequest();
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

        // PUT api/v1/Requests/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] CourseRequestStatus status)
        {
            Request? request = await _unitOfWork.Request.GetByIdAsync(id);
            if (request == null)
            {
                return NotFound();
            }
            try
            {
                await _courseRequestService.UpdateRequestStatus(request, status);
            }

            catch (Exception ex)
            {
                return BadRequest();
            }
            return Ok();
            
        }
    }
}
