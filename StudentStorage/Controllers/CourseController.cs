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
    public class CourseController : ControllerBase
    {

        IUnitOfWork _unitOfWork;
        UserManager<ApplicationUser> _userManager;
        IMapper _mapper;

        public CourseController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;

        }

        // GET api/CourseController
        [HttpGet]
        [Authorize(Roles = UserRoles.Student)]
        public async Task<IActionResult> Get()
        {
            var courses = await _unitOfWork.Course.GetAllAsync(includeProperties: "Creator");
            IEnumerable<CourseResponseDTO> courseResponseDTOs = courses.Select(_mapper.Map<CourseResponseDTO>);
            return Ok(courseResponseDTOs);
        }

        // GET api/CourseController/5
        [HttpGet("{id}")]
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

        // POST api/CourseController
        [HttpPost]
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

        // PUT api/CourseController/5
        [HttpPut("{id}")]
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

        // DELETE api/CourseController/5
        [HttpDelete("{id}")]
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
