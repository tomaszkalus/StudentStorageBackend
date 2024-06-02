using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentStorage.DataAccess.Repository.IRepository;
using StudentStorage.Models;
using StudentStorage.Services;

namespace StudentStorage.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SolutionsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IAuthorizationService _authorizationService;
        private readonly AssignmentSolutionService _assignmentSolutionService;

        public SolutionsController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IMapper mapper, IAuthorizationService authorizationService, AssignmentSolutionService assignmentSolutionService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
            _authorizationService = authorizationService;
            _assignmentSolutionService = assignmentSolutionService;
        }

        // GET api/v1/Solution/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var solution = await _unitOfWork.Solution.GetByIdAsync(id);
            if (solution == null)
            {
                return NotFound();
            }

            Course? course = await _unitOfWork.Course.GetByIdAsync(solution.Assignment.CourseId);

            if (course == null)
            {
                return NotFound();
            }

            var authorizationResult = await _authorizationService
            .AuthorizeAsync(User, course, "CourseMembershipPolicy");

            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }
            return Ok();

            // get solution file


        }
        


    }
}
