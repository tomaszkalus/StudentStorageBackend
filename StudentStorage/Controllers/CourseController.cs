using BookStoreMVC.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentStorage.Models;
using StudentStorage.Models.Authentication;
using StudentStorage.Models.DTO;
using StudentStorage.Models.Responses;
using System.Security.Claims;

namespace StudentStorage.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {

        IUnitOfWork _unitOfWork;
        UserManager<ApplicationUser> _userManager;

        public CourseController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;

        }

        // GET: api/CourseController
        [HttpGet]
        public IEnumerable<Course> Get()
        {
            return _unitOfWork.Course.GetAll();
        }

        // GET api/CourseController/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {

            Course? course = _unitOfWork.Course.GetById(id);
            if (course == null)
            {
                return BadRequest();
            }
            return Ok(course);
        }

        // POST api/CourseController
        [HttpPost]
        [Authorize(Roles = UserRoles.Teacher)]
        public async Task<ActionResult> Post([FromBody] CourseDTO courseDTO)
        {
            ApplicationUser? currentUser = await _userManager.GetUserAsync(HttpContext.User);
            Course course = new Course
            {
                CreatorId = currentUser.Id,
                Name = courseDTO.Name,
                Description = courseDTO.Description,
                CreatedAt = DateTime.Now
            };
            _unitOfWork.Course.Add(course);
            _unitOfWork.Save();
            return Ok(course);
        }

        // PUT api/CourseController/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] CourseDTO courseDTO)
        {
            Course? course = _unitOfWork.Course.GetById(id);
            if (course == null)
            {
                return BadRequest();
            }   
            course.Name = courseDTO.Name;
            course.Description = courseDTO.Description;
            _unitOfWork.Course.Update(course);
            _unitOfWork.Save();
            return Ok(course);
        }

        // DELETE api/CourseController/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            Course? course = _unitOfWork.Course.GetById(id);
            if (course == null)
            {
                return NotFound();
            }
            _unitOfWork.Course.Remove(course);
            _unitOfWork.Save();
            return Ok();
        }
    }
}
