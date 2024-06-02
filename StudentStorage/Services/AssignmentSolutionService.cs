using StudentStorage.DataAccess.Repository.IRepository;
using StudentStorage.Models;
using StudentStorage.Models.DTO.Solution;

namespace StudentStorage.Services
{
    public class AssignmentSolutionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly FileManagerService _fileManagerService;

        public AssignmentSolutionService(IUnitOfWork unitOfWork, FileManagerService fileManagerService)
        {
            _unitOfWork = unitOfWork;
            _fileManagerService = fileManagerService;
        }

        public async Task<ServiceResult> SubmitAssignmentSolutionAsync(SolutionRequestDTO solutionRequest, Assignment assignment, ApplicationUser user)
        {
            string? filePath = _fileManagerService.AddAssignmentSolutionFiles(solutionRequest.File, assignment, user);
            if (filePath == null)
            {
                return new ServiceResult(false, "Could not save solution files.");
            }

            Solution solution = new Solution
            {
                CreatorId = user.Id,
                AssignmentId = assignment.Id,
                FilePath = filePath,
                Description = solutionRequest.Description,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await _unitOfWork.Solution.AddAsync(solution);
            await _unitOfWork.CommitAsync();

            return new ServiceResult(true, "Assignment solution submitted successfully.");
        }

        //public async Task<ServiceResult> GetUserSolution(int solutionId, ApplicationUser user)
        //{
        //    Solution? solution = await _unitOfWork.Solution.GetByIdAsync(solutionId);
        //    if (solution == null)
        //    {
        //        return new ServiceResult(false, "Solution not found.");
        //    }

        //    if (solution.CreatorId != user.Id)
        //    {
        //        return new ServiceResult(false, "You are not the creator of this solution.");
        //    }

        //    return new ServiceResult(true, "Solution found.", solution);
        //}
    }
}
