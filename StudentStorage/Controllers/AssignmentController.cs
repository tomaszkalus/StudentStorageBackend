using AutoMapper;
using BookStoreMVC.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentStorage.Models;
using StudentStorage.Models.Authentication;
using StudentStorage.Models.DTO;

namespace StudentStorage.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AssignmentController : ControllerBase
    {
        IUnitOfWork _unitOfWork;
        UserManager<ApplicationUser> _userManager;
        IMapper _mapper;

        public AssignmentController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;

        }

        // GET api/AssignmentController
        [HttpGet]
        [Authorize(Roles = UserRoles.Student)]
        public async Task<IActionResult> Get()
        {
            var Assignments = await _unitOfWork.Assignment.GetAllAsync(includeProperties: "Creator");
            IEnumerable<AssignmentResponseDTO> AssignmentResponseDTOs = Assignments.Select(_mapper.Map<AssignmentResponseDTO>);
            return Ok(AssignmentResponseDTOs);
        }

        // GET api/AssignmentController/5
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

        // POST api/AssignmentController
        [HttpPost]
        [Authorize(Roles = UserRoles.Teacher)]
        public async Task<ActionResult> Post([FromBody] AssignmentRequestDTO AssignmentDTO)
        {
            ApplicationUser? currentUser = await _userManager.GetUserAsync(HttpContext.User);
            Assignment Assignment = new Assignment
            {
                Title = AssignmentDTO.Title,
                Description = AssignmentDTO.Description,
                DueDate = AssignmentDTO.DueDate,
                AllowLateSubmissions = AssignmentDTO.AllowLateSubmissions,
                Hidden = AssignmentDTO.Hidden,
            };

            _unitOfWork.Assignment.AddAsync(Assignment);
            await _unitOfWork.SaveAsync();
            AssignmentResponseDTO AssignmentResponseDTO = _mapper.Map<AssignmentResponseDTO>(Assignment);
            return Ok(AssignmentResponseDTO);
        }

        // PUT api/AssignmentController/5
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
            await _unitOfWork.SaveAsync();
            AssignmentResponseDTO AssignmentResponseDTO = _mapper.Map<AssignmentResponseDTO>(Assignment);
            return Ok(AssignmentResponseDTO);
        }

        // DELETE api/AssignmentController/5
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
            await _unitOfWork.SaveAsync();
            return Ok();
        }
    }
}
