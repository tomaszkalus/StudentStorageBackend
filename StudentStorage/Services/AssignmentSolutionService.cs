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

        public async Task<ServiceResult> SubmitAssignmentSolutionFilesAsync(SolutionRequestDTO solutionRequest, Assignment assignment, ApplicationUser user)
        {
            
            var solutions = _fileManagerService.SaveSolutionFiles(solutionRequest.Files, assignment, user);

            if (solutions == null)
            {
                return new ServiceResult(false, "Failed to save solution files.");
            }

            foreach(var solution in solutions)
            {
                await _unitOfWork.Solution.AddAsync(solution);
            }

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
