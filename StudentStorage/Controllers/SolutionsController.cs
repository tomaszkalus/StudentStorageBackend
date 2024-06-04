using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using StudentStorage.DataAccess.Repository.IRepository;
using StudentStorage.Models;
using StudentStorage.Models.Authentication;
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

        /// <summary>
        /// Gets a specific solution file by ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A solution file with the given ID</returns>
        // GET api/v1/Solutions/5
        [HttpGet("{id}")]
        [Authorize(Roles = UserRoles.Student)]
        public async Task<IActionResult> Get(int id)
        {
            Solution? solution = await _unitOfWork.Solution.GetByIdAsync(id);
            if (solution == null)
            {
                return NotFound();
            }
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser.Id != solution.CreatorId)
            {
                return Forbid();
            }

            IFormFile file = _assignmentSolutionService.GetAssignmentSolutionFile(solution);

            if (file == null)
            {
                return NotFound();
            }

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(file.FileName, out var mimeType))
            {
                mimeType = "application/octet-stream";
            }

            return File(file.OpenReadStream(), mimeType, file.FileName);
        }

        /// <summary>
        /// Deletes a specific solution file by ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>200 if the operation was successful, 404 if solution was not found and 403 if user is not the solution creator.</returns>
        // DELETE api/Solutions/5
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpDelete("{id}")]
        [Authorize(Roles = UserRoles.Student)]
        public async Task<IActionResult> DeleteSolution(int id)
        {
            Solution? solution = await _unitOfWork.Solution.GetByIdAsync(id);
            if (solution == null)
            {
                return NotFound();
            }
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser.Id != solution.CreatorId)
            {
                return Forbid();
            }

            var result = await _assignmentSolutionService.DeleteAssignmentSolutionFileAsync(id, currentUser);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok();
        }



    }
}
