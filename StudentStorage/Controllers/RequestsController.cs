using AutoMapper;
using BookStoreMVC.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentStorage.Models;
using StudentStorage.Models.Authentication;
using StudentStorage.Models.DTO;
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

        public RequestsController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IMapper mapper, CourseRequestService courseRequestService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
            _courseRequestService = courseRequestService;

        }

        // GET api/v1/Requests/5
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [Authorize(Roles = UserRoles.Student)]
        public async Task<IActionResult> Get(int id)
        {
            Request? request = await _unitOfWork.Request.GetByIdAsync(id);
            if (request == null)
            {
                return BadRequest();
            }
            RequestResponseDTO requestResponseDTO = _mapper.Map<RequestResponseDTO>(request);
            return Ok(requestResponseDTO);
        }

        // PUT api/v1/Requests/5
        [HttpPut("{id}")]
        public async void Put(int id, [FromBody] CourseRequestStatus status)
        {
            Request? request = await _unitOfWork.Request.GetByIdAsync(id);
            if (request == null)
            {
                return;
            }
            request.Status = status;
            await _courseRequestService.UpdateRequestStatus(request);
        }

    }
}
